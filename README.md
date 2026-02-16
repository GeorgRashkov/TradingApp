# **Trading app for 3D models**
This project is for learning purposes and it is focused on showing the main characteristics of an ASP.NET MVC layered architecture. The app allows users to trade products and the main subject for trading is virtual 3D models. The  main trading subject is only an idea and the app will work with images (type `.jpg`) instead of actual 3D model files. The purpose for trading is that when a user buys a product he will be able to dowload the main traiding subject on his PC while the product's creator balance will increase. 


### **Features**

* User authentication and authorization
* Allows users to create products
* Allows users to create sell orders of the products they created
* Allows users to edit their products
* Allows users to delete their products
* Allows users to browse and buy products created by other users
* Allows users to track their completed orders
* Allows buyers to download the products they purchased


<br>

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


### **Confirmation pages**

Those pages provide a desription of what will happen if the user confirms and provide 2 buttons - one for cancellation and one for confirmation. The cancellation button redirects the user the previous page. Even though all confirmation pages look the same the action method of the `confirm` button is specific for each page. There are 4 confirmation pages:
* `BuySellOrder` - displays when a user tries to buy a product
* `CreateSellOrder` - displays when a user tries to create one or many sell orders of one of his products
* `CancelSellOrder` - displays when a user tries to cancel one or many sell orders of one of his products
* `DeleteProduct` -  displays when a user tries to delete one of his products

### **Message**

This page is used by other pages for showing messages of different type - error, success, hint, etc.

### **Invoices**

This page shows information about the successfully purchased or sold products. The data is displayed in a table which has 3 columns - `Title` which provides a small description of the order, `Completed At` which shows the date and time when the product was pruchased/soled, `Details` contains a button details which will show additional data for the order. The page will extract a set number of invoices from the DB. The invoices will be ordered in descendinding order by their completion date. The user can navigate through the invoices by pressing the left and right arrow buttons on the bottom of the page.

### **Invoice**

This page shows some additional information about the invoice such as the `front` image of the 3D model and the money which the user gained/spend for the product. If the product was purchased by the user the `Invoice` page will also show the seller's username and a `dowload` button which when pressed will download the 3D model file on the user's computer. The `dowload` button will not appear if the creator of the product deletes the product. 

### **Balance**

This page is supposed to represent a money transfer from the user's bank account to the app's DB Balance table and vice versa. Since the application doesn't have bank accounting algorithm this page simple allows the user to increase or decrease his balance as much as he wants. The reason this page exists is because when a new user registers he will have no money in his balance which means he would not be able to buy products unless he manages to create and sell one or more products to get the money he needs to buy the desired one.  

<br>

## **How to use**

### **Register and login**

If you are not logged in you will have access only to the `Products` and `Product` pages and you will not be able to buy products. You can create a new user by going to the register page and fill the required fields. After the creation you have to go the login page and log in. Newly created users have no money in their balance and since their balance will always be below the products' prices the new users will not be able to buy products. In order to increase the user's balance you can create a product with a sell order, log out and log in as another user (who has money in his balance) and buy the product which will increase the balance of the seller and decrease the balance of the buyer. The app takes 10% from the product price as a tax which means the buyer pays the full product's price however the seller get's 90% of the product's price. An easier way to increase or decrease the balance of an existing user is to go to the `Balance` page and specify the amount of money.

### **Create products**

In order to create products you have to go to `MyProducts` page and press the button `Create product` located at the top right corner. After that you will redirected to `CreateProduct` page. You have to fill all text fields with valid data and you have to provide a .jpg files for the file fields. The only required file fields are the first one (which should be a picture showing the front side of the 3D model) and the last one (an image file which we will pretend to be a 3D model file). The front side picture is required because it will appear as a product picture in `MyProducts` and `Products` pages. The last image is needed because when users buy the product they need to be able to dowload the 3D model (in this case the picture that pretends to be the 3D model) they spent their money for. All other image files are optional but recommended to fill becuase they will show the way the 3D model looks from the other 5 out 6 main sides (the users will be able to see those pictures in `Product` page). When you fill all required fields with valid data and submit the form the app will create the product and will redirect you to the `Message` page (the app will not create the product if the user already has a product with the same name). Once you create the product you will be able to find it in `MyProducts` page and if you want to see some additional data about it you can click the button `View` for the product which will redirect you to `MyProduct`. Since the newly created product doesn't have any sell orders you will not be able to find it in `Products` page. 

### **Update product**

In order to update a product you have to go to `MyProducts` page and press the `edit` button for the product you want to edit. If the logged user has active sell order/s of the selected product the app will not allow you to edit the product and will redirect you to the `Message` page. If this happens you can either cancel all sell orders of the product or if the product has only one active sell order you can log out and log in as another user, buy the product and then log out and log in as the first user. If the selected product doesn't have any active sell orders after you press the `edit` button the app will redirect you to `UpdateProduct` page. The only required fields are the text fields which will be filled automatically with the product's data. For the file fields you should either provide .jpg files or provide nothing at all. The file fields which are left empty will not alter the existing image files while a file field for which you provide a file will overring the existing one. When you fill all required fields with valid data and submit the form the app will apply your changes to the product and will redirect you to the `Message` page. You can check the changes in the pages `MyProducts` and `MyProduct`.

### **Create sell order**

In order to create a sell order you have to go to `MyProducts` choose a product and press the `View` button which will redirect you to the product's details. At the bottom of the page you should be able to see a button `Create sell order/s` and a text field before it. The app will not show those elements if the logged user has too many active sell orders or if the product status is not `approved` but it will provide some text on their place explaining the reason you are not allowed to create sell orders of the product. If the app shows the button `Create sell order/s` and it's text field you can either enter a number in the field (which indicates the ammount of sell orders you want to create) or leave it empty and press the button. The app will then redirect you to the confirmation page `CreateSellOrder` where you will be able to see the amount of sell orders you are about to create (the app will automatically lower their number if the number of active sell orders of the user combined with the number you entered exceeds the maximum number of active sale orders). Once you confirm the app will create the sell orders (their status will be `active`) you specified and will redirect to the `Message` page. You can find the current amount of active sell orders of the product in `MyProduct` and `Product` pages.

### **Cancel sell order**

Cancelling a sell order is very similar to creating one. The cancellation option is presented right above the creation one. The app will not show it if the product you seleted has no active sell orders. If you see the cancellation option you do the same thing a for the creation option - you enter a number (or nothing) and than press the button `Cancel sell order/s`. The app then redirects you to the confirmation page `CancelSellOrder` and after you confirm the app will canell the specified amount of active sell orders of the product and will redirect you to the `Message` page. Cancelling an active sale order does not delete the order - it only set's the status of the order to `cancelled`.

### **Delete product**

In order to delete a product you have to go to `MyProducts` page and press the `delete` button for the product you want to delete. The app then redirects you to the confirmation page `DeleteProduct`. After you confirm, the app will delete the product from the DB as well as it's sell orders (completed orders are not touched).

### **Buy products**

In order to buy products you have to go to `Products` page and press the `Buy` button for the product you want to purchase. If the logged user doesn't have enough money or if he is the creator of the product or if he already purchased the product you will be redirected to the `Message` page where you will be able to see the reason the app is not allowing you to buy the product. Otherwise you will be redirected to the confirmation page `BuySellOrder`. If you confirm the app will create a completed order record (which will be used for creating an invoice), it will set status of the sell order to `completed` and will redirect you to the `Message` page. In order to download the 3D model file (in this case the picture that pretends to be the 3D model) you purchased you have to go to `Invoices` page, click the button `Details` (which will redirect you to `Invoice` page) for the desired invoice (since they are order in descending order by creation date the last order you created will be the first one to appear) and then click the button `Download`. In the `Invoice` page you will be able to see the seller's Username and if you log out and log in as the seller and go to the `Invoices` page you will find the same invoice in there, however the title will be a bit different and in the `Invoice` page will see that the seller Username and dowload button are missing. The reason the download button is not presented in the seller's invoice page is becuase the seller is always the creator of the product which means the 3D model file (the picture that pretends to be the 3D model) was uploaded by him so he doesn't need to dowload it again.

### **Increase, decrease balance** ##

You can quickly increase or decrease the balance of a logged user by going to the `Balance` page. In order to increase the balance you have to enter a number in the text field `Increase balance` and press the button `Increase balance`. After you press the button the app will redirect you to the `Message` page showing the user's balance after the increment operation. Decreasing the balance works in the same way as increasing it (just use the text field and button `Decrease balance`).