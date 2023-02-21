﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Restaurant_rating_Mobile
{
    public class Menuofrestaurant
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string Photo { get; set; }
    }
}
