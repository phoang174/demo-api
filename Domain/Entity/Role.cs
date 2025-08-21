﻿namespace Domain.Entity
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public List<UserRole> UserRole { get; } = [];

    }
}
