using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Restaurant_rating_Mobile
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public int AverageBill { get; set; }
        public string Address { get; set; }
        public string WebSiteLink { get; set; }
        public int IsClosed { get; set; }
        public int OwnerId { get; set; }
    }
}
