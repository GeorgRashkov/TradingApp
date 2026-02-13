using TradingApp.GCommon.Enums;

namespace TradingApp.Common
{
    //this class will be used for filtering products when searching for them in the database;
    public class ProductFilter
    {
        //determines the product status which will be used when searching for products;
        //if the value is null the products will not be filtered by their status
        public ProductStatus? ProductStatus { get; set; }

        //determines the sell order status which will be used when searching for products;
        //if a product has many sell orders it will be selected only if at least one of the sell order statuses matches the value of `SellOrderStatus`;
        //if a product has no sell orders it will not be selected ;
        //if the value is null the products will not be filtered by their sell order status (products with no sell orders will be selected);
        public SellOrderStatus? SellOrderStatus { get; set; }

        //determines the product id which will be used when searching for products;
        //since each product has unique id the filter will select at most one product;
        //if the value is null the products will not be filtered by their id
        public Guid? PorductId { get; set; }

        //determines the user id of the creator which will be used when searching for products;
        //if the value is null the products will not be filtered by their creator's id
        public string? UserId { get; set; }


        //determines the username of the creator which will be used when searching for products;
        //if the value is null the products will not be filtered by their creator's username
        public string? Username { get; set; }

        //determines the product name which will be used when searching for products;
        //if the value is null the products will not be filtered by their name
        public string? ProductName { get; set; }

        //this property will be used for filtering only if the `Username` property is not null;
        //if the value is `true` all products whose creator's username contains the value of the `Username` property will be selected;
        //if the value is `false` only products whose creator's username is exactly the same as the value of the `Username` property will be selected
        public bool UsernameContains { get; set; } = false;


        //this property will be used for filtering only if the `ProductName` property is not null;
        //if the value is `true` all products whose product name contains the value of the `ProductName` property will be selected;
        //if the value is `false` only products whose name is exactly the same as the value of the `ProductName` property will be selected
        public bool ProductNameContains { get; set; } = false;


        //determines the number of products which will be skipped when searching for products in the database;
        //if the value is null no products will be skipped;
        private int? _skip = null;
        public int? Skip 
        { 
            get { return _skip; } 
            set 
            {
                if (value is not null)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Skip));
                    }
                }

                _skip = value;
            } 
        }


        //determines the maximum number of products which will be selected when searching for products in the database;
        //if the value is null all found products will be taken
        private int? _take = null;
        public int? Take
        {
            get { return _take; }
            set
            {
                if (value is not null)
                {
                    if (value < 0)
                    { 
                        throw new ArgumentOutOfRangeException(nameof(Take)); 
                    }
                }

                _take = value;
            }
        }
    }
}
