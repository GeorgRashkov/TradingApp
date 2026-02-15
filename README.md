# **Trading app for 3D models**
This project is for learning purposes and it is focused on showing the main characteristics of an ASP.NET MVC layered architecture. The app allows users to trade products and the main subject for trading is virtual 3D models. The  main trading subject is only an idea and the app will work with images (type `.jpg`) instead of actual 3D model files. The purpose for trading is that when a user buys a product he will be able to dowload the main traiding subject on his PC while the product's creator balance will increase. 


## **Features**

* User authentication and authorization
* Allows users to create products
* Allows users to create sell orders of the products they created
* Allows users to edit their products
* Allows users to delete their products
* Allows users to browse and buy products created by other users
* Allows users to track their completed orders
* Allows buyers to download the products they purchased



## **Pages**

### **Products**

Shows all products which have state `approved` and also have at least one sell order. In order to lower the amount of data which goes from the DB to the client only a set number of products are shown. The user can navigate through the products by pressing the left and right arrow buttons on the bottom of the page. For each product the user can see the `front` image of the 3D model and also it's name, price and creator Username. The user can buy the product and can also check it's details before bying it. The app will not allow users to buy a product if their balance is below the product price nor if they already purchased the product. Creators are also not allowed to buy their own products. 


### **Product**

Shows additional information about the product such as the number of items in stock, a description and six images showing the 3D model captured from the 6 main sides - `front`, `back`, `top`, `bottom`, `left`, `right`. The page allows the users the buy the product.


### **MyProducts**

Shows all products created by the curretly logged user. This page looks a lot like the `Products` page however instead of providing a `buy` option it allows the user to edit and delete the selected product. The page also has a button `Create product` which allows the user create a new product. Since the creator is always the curretly logged user, for each product the page shows it's status instead of the username of the creator. 


### **MyProduct**

Shows additional information about the product created by the currently logged user such as the number of active sale orders, the product description and six images of the 3D model. The user can edit and delete the product, and he can also create and cacel sell orders of the product. The app will not allow the user to create a sell order if the product's status is not `approved`. 


### **CreateProduct**

This page is a form which has 3 text fields and 7 file fields. In the text fields the user must provide information about the product's name, description and price. For the first 6 file fields the user should upload 6 images showing the 3D model from the 6 main sides (only the front side picture is required). For the last file field the user must provide the file containing the 3D model. The required fields are the text fields, the `fron` image, the 3D model file. The app will not allow the user to create a product if he already created a product with the same name.


### **UpdateProduct**

This page looks a lot like `CreateProduct` however the purpose of this page is to edit an existing product rather than creating a new one. The text fields are automatically filled with the product's data. All file fields are optional and those for which the user provides a file will override the already existing one.  


## **Confirmation pages**

Those pages provide a desription of what will happen if the user confirms and provide 2 buttons - one for cancellation and one for confirmation. The cancellation button redirects the user the previous page. Even though all confirmation pages look the same the action method of the `confirm` button is specific for each page. There are 4 confirmation pages:
* `BuySellOrder` - displays when a user tries to buy a product
* `CreateSellOrder` - displays when a user tries to create one or many sell orders of one of his products
* `CancelSellOrder` - displays when a user tries to cancel one or many sell orders of one of his products
* `DeleteProduct` -  displays when a user tries to delete one of his products


## **Invoices**

This page shows information about the successfully purchased or sold products. The data is displayed in a table which has 3 columns - `Title` which provides a small description of the order, `Completed At` which shows the date and time when the product was pruchased/soled, `Details` contains a button details which will show additional data for the order. The page will extract a set number of invoices from the DB. The invoices will be ordered in descendinding order by their completion date. The user can navigate through the invoices by pressing the left and right arrow buttons on the bottom of the page.

## **Invoice**

This page shows some additional information about the invoice such as the `front` image of the 3D model and the money which the user gained/spend for the product. If the product was purchased by the user the `Invoice` page will also show the seller's username and a `dowload` button which when pressed will download the 3D model file on the user's computer. The `dowload` button will not appear if the creator of the product deletes the product. 

## **Balance**

This page is supposed to represent a money transfer from the user's bank account to the app's DB Balance table and vice versa. Since the application doesn't have bank accounting algorithm this page simple allows the user to increase or decrease his balance as much as he wants. The reason this page exists is because when a new user registers he will have no money in his balance which means he would not be able to buy products unless he manages to create and sell one or more products to get the money he needs to buy the desired one.  