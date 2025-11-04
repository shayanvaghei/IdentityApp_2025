namespace API.DTOs.Pagination
{
    public class AllUsersParams : BaseParams
    {
        private string _scvRoles;
        public string CsvRoles
        {
            get => _scvRoles;
            set => _scvRoles = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }

        private string _lockUnlock;
        public string LockUnlock
        {
            get => _lockUnlock;
            set => _lockUnlock = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }

        private string _activation;
        public string Activation
        {
            get => _activation;
            set => _activation = string.IsNullOrEmpty(value) ? "" : value.ToLower();
        }
    }
}
