using System.Management;
using System.Security.Cryptography;
using System.Text;
namespace EmailExtractionFrmSites_Tool
{
  
    public static class DeviceLockHelper
    {
        public static string GetMachineID()
        {
            try
            {
                using (var mc = new ManagementClass("Win32_ComputerSystemProduct"))
                {
                    foreach (var o in mc.GetInstances())
                    {
                        var mo = (ManagementObject)o;
                        return mo["UUID"].ToString();
                    }
                }
            }
            catch { }
            return Environment.MachineName; // fallback
        }
    }
    public static class TrialHelper
    {
        private static string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "namoritrial.dat");
        private static int trialDays = 30;

        public static bool IsTrialExpiredOrDeviceMismatch()
        {
            string machineId = DeviceLockHelper.GetMachineID();

            if (File.Exists(filePath))
            {
                try
                {
                    string decrypted = DecryptFile(filePath);
                    var parts = decrypted.Split('|');
                    if (parts.Length == 2 &&
                        DateTime.TryParse(parts[0], out DateTime startDate) &&
                        parts[1] == machineId)
                    {
                        return  (DateTime.Now - startDate).TotalDays > trialDays;
                    }
                    else
                    {
                        return true; // tampered or mismatch
                    }
                }
                catch
                {
                    return true; // decryption failure = tampered
                }
            }

            // First run: create file
            string data = $"{DateTime.Now}|{machineId}";
            EncryptAndSave(filePath, data);
            return false;
        }

        public static int RemainingDays()
        {
            if (!File.Exists(filePath)) return trialDays;

            try
            {
                string decrypted = DecryptFile(filePath);
                var parts = decrypted.Split('|');
                if (parts.Length == 2 && DateTime.TryParse(parts[0], out DateTime startDate))
                {
                    int usedDays = (int)(DateTime.Now - startDate).TotalDays;
                    return Math.Max(0, trialDays - usedDays);
                }
            }
            catch
            {
                // corrupted file
            }

            return 0;
        }

        public static bool UpdateExpiryWithKey(string base64Key)
        {
            try
            {
                string decodedDate = Encoding.UTF8.GetString(Convert.FromBase64String(base64Key));
                if (!DateTime.TryParseExact(decodedDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime newDate))
                    return false;

                string machineId = DeviceLockHelper.GetMachineID();
                string data = $"{newDate}|{machineId}";
                EncryptAndSave(filePath, data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void EncryptAndSave(string path, string plainText)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(path, encrypted);
        }

        private static string DecryptFile(string path)
        {
            byte[] encrypted = File.ReadAllBytes(path);
            byte[] decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }
    }

 
}
