﻿namespace CoffeeShop.Models
{
    public class Coffee
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public BeanVariety beanVariety { get; set; }

    }
}
