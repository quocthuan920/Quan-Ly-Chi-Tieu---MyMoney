namespace MyMoney.Application.Common.Constants
{
    /// <summary>
    /// Contains constant values regarding the database
    /// </summary>
    public static class DatabaseConstants
    {
        /// <summary>
        /// Name of the Backup Folder
        /// </summary>
        public static string ARCHIVE_FOLDER_NAME => "Archive";

        /// <summary>
        /// Name of the database backup
        /// </summary>
        public static string BACKUP_NAME => "backupMyMoney3.db";

        /// <summary>
        /// Name of the database backup archive
        /// </summary>
        public static string BACKUP_ARCHIVE_NAME => "backupMyMoney3_{0}.db";
    }
}
