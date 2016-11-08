using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ForensicIOS
{
    internal class DeviceExtractor
    {
        private string _backupDevicePath;
        private string _backupPath;

        public void Start()
        {
            Console.WriteLine("Starting IDeviceExtractor...");
            GenerateBackupPath();
            PerformFullBackup();
            DumpWhatsapp();
            DumpAddressBook();
            DumpSMS();
            DumpSafari();
            CleanUp();
            Console.WriteLine("Finished");
        }

        private string GetFileIdFromDomainandRelativePath(string domain, string relativePath)
        {
            var dbConnection =
                new SQLiteConnection("Data Source=" + _backupPath +
                                     "\\33af4fdf827889bb8d10d44b1029cf5013211780\\Manifest.db;Version=3;");
            dbConnection.Open();

            var sql = "select fileId from Files WHERE domain = '" + domain + "' AND relativePath = '" + relativePath +
                      "'";
            var command = new SQLiteCommand(sql, dbConnection);
            var reader = command.ExecuteReader();
            reader.Read();

            var fileId = (string) reader["fileId"];

            dbConnection.Close();

            return fileId;
        }

        private void DumpWhatsapp()
        {
            Console.WriteLine("... Whatsapp");
            var whatsappFileKey = GetFileIdFromDomainandRelativePath(
                "AppDomainGroup-group.net.whatsapp.WhatsApp.shared", "ChatStorage.sqlite");

            var whatappDir = whatsappFileKey.Substring(0, 2);
            var whatsappFile = _backupDevicePath + whatappDir + "\\" +
                               whatsappFileKey;

            if (Directory.Exists(Environment.CurrentDirectory + "\\dumps\\dump-whatsapp"))
                Directory.Delete(Environment.CurrentDirectory + "\\dumps\\dump-whatsapp", true);
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\dumps\\dump-whatsapp");

            File.Move(whatsappFile, Environment.CurrentDirectory + "\\dumps\\dump-whatsapp\\ChatStorage.sqlite");
        }

        private void DumpAddressBook()
        {
            Console.WriteLine("... AddressBook");
            var addrFileKey = GetFileIdFromDomainandRelativePath("HomeDomain",
                "Library/AddressBook/AddressBook.sqlitedb");

            var addrDir = addrFileKey.Substring(0, 2);
            var addrFile = _backupDevicePath + addrDir + "\\" +
                           addrFileKey;

            if (Directory.Exists(Environment.CurrentDirectory + "\\dumps\\dump-addressbook"))
                Directory.Delete(Environment.CurrentDirectory + "\\dumps\\dump-addressbook", true);
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\dumps\\dump-addressbook");

            File.Move(addrFile, Environment.CurrentDirectory + "\\dumps\\dump-addressbook\\AddressBook.sqlite");
        }

        private void DumpSMS()
        {
            Console.WriteLine("... SMS");
            var smsFileKey = GetFileIdFromDomainandRelativePath("HomeDomain",
               "Library/SMS/sms.db");

            var smsDir = smsFileKey.Substring(0, 2);
            var smsFile = _backupDevicePath + smsDir + "\\" +
                           smsFileKey;

            if (Directory.Exists(Environment.CurrentDirectory + "\\dumps\\dump-sms"))
                Directory.Delete(Environment.CurrentDirectory + "\\dumps\\dump-sms", true);
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\dumps\\dump-sms");

            File.Move(smsFile, Environment.CurrentDirectory + "\\dumps\\dump-sms\\SMS.sqlite");
        }

        private void CleanUp()
        {
            Directory.Delete(_backupPath, true);
        }
        private void DumpSafari()
        {
            Console.WriteLine("... Safari");
            var smsFileKey = GetFileIdFromDomainandRelativePath("HomeDomain",
               "Library/Safari/Bookmarks.db");

            var smsDir = smsFileKey.Substring(0, 2);
            var smsFile = _backupDevicePath + smsDir + "\\" +
                           smsFileKey;

            if (Directory.Exists(Environment.CurrentDirectory + "\\dumps\\dump-safari"))
                Directory.Delete(Environment.CurrentDirectory + "\\dumps\\dump-safari", true);
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\dumps\\dump-safari");

            File.Move(smsFile, Environment.CurrentDirectory + "\\dumps\\dump-safari\\Safari.sqlite");
        }

        private void GenerateBackupPath()
        {
            Console.WriteLine("Initializing backup i/o");
            _backupPath = Environment.CurrentDirectory + "\\backup";
            _backupDevicePath = _backupPath + "\\33af4fdf827889bb8d10d44b1029cf5013211780\\";

            if (Directory.Exists(_backupPath))
            {
                Directory.Delete(_backupPath, true);
                Thread.Sleep(100);
            }


            Directory.CreateDirectory(_backupPath);
            Thread.Sleep(100);
        }

        private void PerformFullBackup()
        {
            Console.WriteLine("Performing a full backup of your idevice...");
            var cmd = new Process();
            cmd.StartInfo.FileName = "idevicebackup2.exe";
            cmd.StartInfo.Arguments = "backup --full --debug \"" + _backupPath + "\"";
            cmd.Start();
            cmd.WaitForExit();
        }
    }
}