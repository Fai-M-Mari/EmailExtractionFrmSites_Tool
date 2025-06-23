using CsvHelper;
using CsvHelper.Configuration;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Page = PuppeteerSharp.Page;

namespace EmailExtractionFrmSites_Tool
{
    public class ExtractEmails
    {
    
        static readonly string[] PathsToCheck =
        {
        "/",
        "/contact/",
        "/contact-us/",
        "/about/",
        "/about-us/",
        "/privacy/",
        "/privacy-policy/",
        "/policy/",
        "/support/",
        "/terms-and-conditions/",
        "/get-in-touch/",
        "/reach-us/",
        "/work-with-us/",
        "/work-with-me/",
        "/who-are-we/",
        "/terms/",
        "/team/",
        "/connect/",
        "/staff/",
        "/directory/"
    };

        public static async Task ProcessFile(string paths,Action<string> reportProgress, Action<int, int> reportCount, CancellationToken cancellationToken)
        {
            reportProgress("🔍 Email Extractor Started");

            var rows = LoadDomainsFromCsv(paths);
            Dictionary<string, string> formData;
            //var rows = LoadDomainsFromCsv(paths, out formData);

            
            if (rows.Count == 0)
            {
                reportProgress("⚠️ No domains found in CSV file.");
                return;
            }

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
            });

            int total = rows.Count;
            int done = 00;
            reportCount(done, total);
            try
            {
                foreach (var row in rows)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!string.IsNullOrWhiteSpace(row.Email))
                    {
                        reportProgress($"⏩ Skipped: {row.Domain}:   ({row.Email})");
                        done++;
                        reportCount(done, total);
                        continue;
                    }

                    string domain = row.Domain;
                    reportProgress($"\n🔍 Checking: {domain}");
                    bool found = false;
                    string foundEmail = null;

                    foreach (var path in PathsToCheck)
                    {
                        string url = $"https://{domain.TrimEnd('/')}{path}";

                        try
                        {
                            var page = await browser.NewPageAsync();
                            await page.GoToAsync(url, WaitUntilNavigation.Networkidle2);
                            await Task.Delay(1000);

                            //// Click the age gate button if it exists
                            //var ageYes = await page.QuerySelectorAsync("button:contains('Yes')");
                            //if (ageYes != null)
                            //{
                            //    await ageYes.ClickAsync();
                            //    await Task.Delay(1500); // wait for page to unlock
                            //}

                            var textContent = await page.EvaluateExpressionAsync<string>("document.body.innerText");
                            var rawHtml = await page.GetContentAsync();

                            var emailFromText = ExtractEmailFromHtml(textContent);
                            var emailFromHtml = ExtractEmailFromHtml(rawHtml);

                            foundEmail = emailFromText ?? emailFromHtml;

                            await page.CloseAsync();

                            //foundEmail = ExtractEmailFromHtml(html);
                            if (!string.IsNullOrEmpty(foundEmail))
                            {
                                reportProgress($"✅ Email Found at {url}:    Email: {foundEmail}");
                                found = true;
                                break;
                            }
                            else
                            {
                                reportProgress($"❌ No email found in HTML snippet at: {url}");
                            }
                        }
                        catch (Exception ex)
                        {
                            reportProgress($"⚠️ Failed to fetch {url}: {ex.Message}");
                        }

                        await Task.Delay(200);
                    }

                    if (!found)
                    {
                        await TryFallbackWithPuppeteerAsync((Browser)browser, domain, row, reportProgress);
                    }
                    else
                    {
                        row.Email = foundEmail;
                    }

                    done++;
                    reportCount(done, total);
                }
            }
            finally
            {
                SaveResultsToCsv(paths, rows, reportProgress);
                await browser.CloseAsync();
            }
        }

        static async Task TryFallbackWithPuppeteerAsync(Browser browser, string domain, EmailResult row, Action<string> reportProgress)
        {
            try
            {
                string homepageUrl = $"https://{domain}";
                var page = await browser.NewPageAsync();
                await page.GoToAsync(homepageUrl, WaitUntilNavigation.Networkidle2);
                var homeHtml = await page.GetContentAsync();
                var contactLinks = FindContactLikeLinks(homeHtml, domain);
                await page.CloseAsync();

                reportProgress($"🔗 Found {contactLinks.Count} Internal links");

                foreach (var contactUrl in contactLinks)
                {
                    try
                    {
                        var subPage = await browser.NewPageAsync();
                        await subPage.GoToAsync(contactUrl, WaitUntilNavigation.Networkidle2);
                        var html = await subPage.GetContentAsync();
                        await subPage.CloseAsync();

                        var foundEmail = ExtractEmailFromHtml(html);
                        if (!string.IsNullOrEmpty(foundEmail))
                        {
                            reportProgress($"✅ Fallback found at {contactUrl}:  {foundEmail}");
                            row.Email = foundEmail;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        reportProgress($"⚠️ Fallback failed for {contactUrl}: {ex.Message}");
                    }

                    await Task.Delay(200);
                }
            }
            catch (Exception ex)
            {
                reportProgress($"⚠️ Homepage fallback error: {ex.Message}");
            }

            row.Email = "Not Found";
        }

        static List<EmailResult> LoadDomainsFromCsv(string filePath)
        {
            var rows = new List<EmailResult>();
            if (!File.Exists(filePath)) return rows;

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var domain = csv.GetField(0)?.Trim(); // first column
                var email = csv.GetField(1)?.Trim();  // second column
                if (!string.IsNullOrWhiteSpace(domain))
                {
                    rows.Add(new EmailResult { Domain = domain, Email = email });
                }
            }

            return rows;
        }

        static List<EmailResult> LoadDomainsFromCsv(string filePath, out Dictionary<string, string> globalFormData)
        {
            var rows = new List<EmailResult>();
            globalFormData = new Dictionary<string, string>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("⚠️ File not found.");
                return rows;
            }

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            if (!csv.Read())
            {
                Console.WriteLine("⚠️ CSV is empty.");
                return rows;
            }

            csv.ReadHeader();
            var headers = csv.HeaderRecord;

            // Detect if file is for form data
            bool isFormData = headers[0].ToLower().Contains("name") || headers[0].ToLower().Contains("field");

            if (isFormData)
            {
                while (csv.Read())
                {
                    var key = csv.GetField(0)?.Trim();
                    var value = csv.GetField(1)?.Trim();

                    if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                    {
                        globalFormData[key.ToLower()] = value;
                    }
                }
            }
            else
            {
                while (csv.Read())
                {
                    var domain = csv.GetField(0)?.Trim();
                    var email = csv.GetField(1)?.Trim();

                    if (!string.IsNullOrWhiteSpace(domain))
                    {
                        rows.Add(new EmailResult { Domain = domain, Email = email });
                    }
                }
            }

            return rows;
        }

        static void SaveResultsToCsv(string filePath, List<EmailResult> results, Action<string> reportProgress)
        {
            using var writer = new StreamWriter(filePath);
            using var csvWriter = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
            csvWriter.WriteHeader<EmailResult>();
            csvWriter.NextRecord();
            foreach (var result in results)
            {
                csvWriter.WriteRecord(result);
                csvWriter.NextRecord();
            }

            reportProgress($"\n\ud83d\udcc1 CSV file saved to: {filePath}");
        }

        static string ExtractEmailFromHtml(string html)
        {
            html = System.Net.WebUtility.HtmlDecode(html);
            html = Regex.Replace(html, "&#(\\d+);", m => ((char)int.Parse(m.Groups[1].Value)).ToString());
            var patterns = new[]
            {
            new Regex("mailto:([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-z]{2,})", RegexOptions.IgnoreCase),
            new Regex("email\\s*[:\\-]?\\s*([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-z]{2,})", RegexOptions.IgnoreCase),
            new Regex("contact\\s*[:\\-]?\\s*([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-z]{2,})", RegexOptions.IgnoreCase),
            new Regex("[a-zA-Z0-9._%+-]+\\s?\\(at\\)\\s?[a-zA-Z0-9.-]+\\s?\\(dot\\)\\s?[a-z]{2,}", RegexOptions.IgnoreCase),
            new Regex("[a-zA-Z0-9._%+-]+\\s?\\[at\\]\\s?[a-zA-Z0-9.-]+\\s?\\[dot\\]\\s?[a-z]{2,}", RegexOptions.IgnoreCase),
            new Regex("[a-zA-Z0-9._%+-]+\\s?@\\s?[a-zA-Z0-9.-]+\\.[a-z]{2,}", RegexOptions.IgnoreCase)
        };

            var matches = new HashSet<string>();

            foreach (var pattern in patterns)
            {
                var found = pattern.Matches(html);
                foreach (Match match in found)
                {
                    var email = match.Groups.Count > 1 ? match.Groups[1].Value : match.Value;
                    email = email.Replace("(at)", "@").Replace("[at]", "@").Replace("(dot)", ".").Replace("[dot]", ".")
                                   .Replace(" ", "").Replace(";", ".").Replace(",", ".");
                    if (IsValidEmail(email)) matches.Add(email);
                }
            }

            return matches.FirstOrDefault();
        }

        static bool IsValidEmail(string email)
        {
            var lower = email.ToLower();
            return Regex.IsMatch(lower, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-z]{2,}$") &&
                   !lower.Contains("example") &&
                   !lower.Contains("noreply") &&
                   !lower.Contains("donotreply") &&
                   !Regex.IsMatch(lower, @"\.(png|jpe?g|gif|svg|webp|bmp|ico)$");
        }

        static List<string> FindContactLikeLinks(string html, string baseDomain)
        {
            var linkPattern = new Regex("href=[\"']([^\"']+)[\"']", RegexOptions.IgnoreCase);
            var matches = linkPattern.Matches(html);
            var links = new HashSet<string>();

            foreach (Match match in matches)
            {
                string link = match.Groups[1].Value;
                if (Regex.IsMatch(link, "contact", RegexOptions.IgnoreCase))
                {
                    if (link.StartsWith("http")) links.Add(link);
                    else if (link.StartsWith("/")) links.Add($"https://{baseDomain}{link}");
                    else links.Add($"https://{baseDomain}/{link}");
                }
            }

            return links.ToList();
        }

        public static async Task<bool> TryAutoFillContactFormAsync(Page page, Action<string> reportProgress, Dictionary<string, string> formData)
        {
            try
            {
                reportProgress("📝 Attempting to auto-fill contact form...");

                var inputElements = await page.QuerySelectorAllAsync("input, textarea, select");

                foreach (var input in inputElements)
                {
                    // Skip hidden elements
                    var isHidden = await input.EvaluateFunctionAsync<bool>("el => el.offsetParent === null || getComputedStyle(el).display === 'none' || el.type === 'hidden'");
                    if (isHidden)
                        continue;

                    var type = await input.EvaluateFunctionAsync<string>("el => el.type || el.tagName.toLowerCase()");
                    var nameAttr = await input.EvaluateFunctionAsync<string>("el => el.getAttribute('name') || ''");
                    var idAttr = await input.EvaluateFunctionAsync<string>("el => el.getAttribute('id') || ''");
                    var placeholder = await input.EvaluateFunctionAsync<string>("el => el.getAttribute('placeholder') || ''");
                    var labelText = await page.EvaluateFunctionAsync<string>(@"
                el => {
                    let label = document.querySelector(`label[for='${el.id}']`);
                    return label ? label.innerText.toLowerCase() : '';
                }", input);

                    string fieldIdentifier = $"{nameAttr} {idAttr} {placeholder} {labelText}".ToLower();

                    string valueToFill = formData.FirstOrDefault(f => fieldIdentifier.Contains(f.Key.ToLower())).Value;

                    if (!string.IsNullOrWhiteSpace(valueToFill))
                    {
                        if (type == "file")
                        {
                            try
                            {
                                await input.UploadFileAsync(valueToFill); // assumes file path is in formData value
                            }
                            catch (Exception ex)
                            {
                                reportProgress($"⚠️ File upload failed: {ex.Message}");
                            }
                        }
                        else
                        {
                            await input.TypeAsync(valueToFill);
                        }
                    }
                    else if (type == "checkbox" || fieldIdentifier.Contains("agree") || fieldIdentifier.Contains("accept"))
                    {
                        await input.ClickAsync();
                    }
                    else if (type == "radio")
                    {
                        await input.ClickAsync();
                    }
                    else if (type == "select-one")
                    {
                        await page.EvaluateFunctionAsync(@"el => {
                    const options = el.options;
                    for (let i = 0; i < options.length; i++) {
                        if (!options[i].disabled) {
                            el.value = options[i].value;
                            break;
                        }
                    }
                }", input);
                    }
                }

                var submitButton = await page.QuerySelectorAsync("button[type='submit'], input[type='submit'], button");
                if (submitButton != null)
                {
                    await submitButton.ClickAsync();
                    reportProgress("🚀 Contact form submitted! ✅");

                    // Wait for confirmation feedback like "Thank you" message
                    try
                    {
                        await page.WaitForFunctionAsync("() => document.body.innerText.toLowerCase().includes('thank you')", new WaitForFunctionOptions { Timeout = 5000 });
                        reportProgress("🎉 Confirmation message detected: 'Thank you'.");
                    }
                    catch
                    {
                        reportProgress("ℹ️ No confirmation message detected.");
                    }

                    return true;
                }
                else
                {
                    reportProgress("⚠️ Submit button not found.");
                }
            }
            catch (Exception ex)
            {
                reportProgress($"❌ Auto-fill failed: {ex.Message}");
            }

            return false;
        }

    }

    public class EmailResult
    {
        public string Domain { get; set; }
        public string Email { get; set; }
    }
}
