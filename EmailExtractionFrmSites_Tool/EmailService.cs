 
using System.Net.Mail;
using System.Net; 
using System.Text.RegularExpressions; 

namespace EmailExtractionFrmSites_Tool
{
    public class EmailService
    {
        public static async Task SendEmail(
    Action<string> reportProgress,
    Action<int, int> reportCount,
    int emailLimit,
    CancellationToken cancellationToken,
    string excelFilePath,
    string rootFolderpath,
    string senderEmail,
    string senderPassword,
    string subject,
    string emailBody,
    int port = 587,
    bool enableSsl = true,
    string smtpClients = "smtp.gmail.com")
        {
            var emailList = ExtractEmails.LoadDomainsFromCsv(excelFilePath, 2);
            if (emailList.Count == 0)
            {
                reportProgress("⚠️ No emails found in CSV file.");
                return;
            }

            int total = emailList.Count;
            int done = 0;
            reportCount(done, total);
            int counter = 0;
            int emaillimitcount = 0;
            if (EmailValidator.IsValidEmail(senderEmail))
            {
                try
                {
                    foreach (var email in emailList)
                    {
                        counter++;
                        
                        cancellationToken.ThrowIfCancellationRequested();
                        string recipientEmail = email.Email?.Trim();

                        if (!EmailValidator.IsValidEmail(recipientEmail))
                        {
                            reportProgress($"❌ Skipped invalid email: {recipientEmail}");
                            email.Status = "Invalid Email";
                            continue;
                        }

                        if (email.Status == "Sent")
                        {
                            done++;
                            reportCount(done, total);
                            reportProgress($"ℹ️ Already sent to: {recipientEmail}");
                            continue;
                        }

                        using (var mail = new MailMessage())
                        {
                            mail.From = new MailAddress(senderEmail);
                            mail.To.Add(recipientEmail);
                            string finalSubject = subject.Replace("{Domain}", email.Domain);
                            mail.Subject = finalSubject;
                            mail.Body = emailBody;

                            using (var smtpClient = new SmtpClient(smtpClients)
                            {
                                Port = port,
                                Credentials = new NetworkCredential(senderEmail, senderPassword),
                                EnableSsl = enableSsl
                            })
                            {
                                try
                                {
                                    await smtpClient.SendMailAsync(mail); // Use async version
                                    reportProgress($"✅ Email sent to {recipientEmail}");
                                    email.Status = "Sent";
                                    done++;
                                    reportCount(done, total);
                                    emaillimitcount++;
                                }
                                catch (Exception ex)
                                {
                                    reportProgress($"❌ Failed to send to {recipientEmail}: {ex.Message}");
                                    email.Status = $"Not Sent: {ex.Message}";
                                    done++;
                                    reportCount(done, total);
                                }
                            }
                        }

                        // Optional: Delay between emails to avoid being blocked (adjust as needed)
                        await Task.Delay(TimeSpan.FromSeconds(10));

                        if (counter == 30)
                        {
                            counter = 0;
                            reportProgress("⏸️ Pausing for 5 minutes after 50 emails...");
                            await Task.Delay(TimeSpan.FromMinutes(5)); // ✅ 5 minutes
                        }

                        if (emaillimitcount >= emailLimit && emailLimit != 0)
                        {
                            reportProgress($"⛔ Email limit reached: {emailLimit}. Stopping process...");
                            break; // ✅ stops the loop completely
                        }
                    }
                }
                catch {
                }
                finally
                { 
                    ExtractEmails.SaveResultsToCsv(excelFilePath, emailList, reportProgress); 
                }
            }
            else
            {
                reportProgress($"❌ Failed: Sender Email is Not Valid: {senderEmail}.\n\n");
                reportProgress($"Please Check the Config file in the Root folder Path {rootFolderpath}.\n\n");
                reportProgress($"Add a valid Email.");
            }
        }

    }

    public static class EmailValidator
    {
        private static readonly string[] InvalidValues =
            { "null", "n/a", "na", "not found", "-", "", "none" };

        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValidEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            string normalized = input.Trim().ToLower();
            if (InvalidValues.Contains(normalized)) return false;

            return EmailRegex.IsMatch(normalized);
        }
    }


} 
public class EmailConfig
{
    public string SmtpClient { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}

