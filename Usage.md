# **Trading app - usage**


<br>

### **Application constants**

The application constants already have default values and you can run the app without changing them but if you don't like the default values you can change them without breaking the app (just make sure the new values match the type of the constant and for date fields make sure you use date formats). Open the project with your IDE and go inside `TradingApp.GCommon\ApplicationConstants.cs`. In there you can find the following constants:

- `InvoicesPerPage` - sets the maximum number of invoices displayed per page
- `ProductsPerPage` - sets the maximum number of products displayed per page
- `RequestsPerPage` - sets the maximum number of requests displayed per page
- `UsersPerPage` - sets the maximum number of users displayed per page
- `ProductReportsPerPage` - sets the maximum number of products reports displayed per page

- `UserMaxActiveSellOrders` - sets the maximum number of active sell orders which the user can have. If the user reaches the maximum number of active sell orders the app will not allow him to create any more sell orders.
- `UserMaxActiveRequests`- sets the maximum number of active order requests which the user can have. If the user reaches the maximum number of active order requests the app will not allow him to create any more order requests.
- `ProductMaxActiveSellOrders` - sets the maximum number of active sell orders which a specific product can have. If the user reaches the maximum number of active sell orders for the product "p" he will not able to create more sell orders of the product "p" however if the user didn't reach the value of `UserMaxActiveSellOrders` he will able to create more sell orders for other products.

- `DateFormat` - specifies the date format
- `DateTimeFormat` - specifies date time format

- `CreatedProductDefaultStatus` - set's the status which will be applied to every newly created product. The status can be `inspection`, `approved`, `disapproved`. In production this value should be set to `inspection` which will not allow the user to create sell orders of newly created products until someone verifies them and set's the product status to `approved` (or if the product doesn't meet the requirements - to `disapproved`). This means whenever a user creates a new product, if you want create sell orders of the product you will have to login as someone who is in role admin or moderator, navigate to the view for product management, find the product and manually set the product status to `approved`. This is the reason it's best to keep the value of `CreatedProductDefaultStatus` to `approved`. 
-`CreatedSellOrderDefaultStatus` - set's the status of newly created sell orders. The status can be `active`, `cancelled`, `completed`. This value should always be set to `active` because buyers can see and purchase only those products which have one or more active sell orders. When a buyer purchases a product the status of the sell order goes from `active` to `completed`. If the seller cancels the sell order before someone buys the product the sell order status goes from `active` to `cancelled`. The app doesn't provide logic for going from `cancelled` or `completed` status to other one.   
-`CreatedOrderRequestDefaultStatus` - set's the status of newly created order requests. When a buyer creates an order requests he is providing description of his 3D model requirements. The status of an order request can be `active`, `cancelled`, `completed`. This value should always be set to `active` because sellers can see and suggest products only to those order requests which are active. The creator of the request can view the 3D model suggestions provided by the sellers. When a user buys a product which was suggested to 2 or more of his order requests the status of the first created order request will be set to `completed`. If the buyer cancels his order request the status goes from `active` to `cancelled`. The app doesn't provide logic for going from `cancelled` or `completed` status to other one.  
-`CreatedProductReportDefaultStatus` - set's the status of newly created product report. Reports allow buyers to report issues with products, while moderators should inspect the reports and determine what to do with the reported products. The report status can be `active`, `in_review`, `resolved`. The role of the product status is only informational - `active` tells moderators that none of them has inspected the report, `in_review` tells moderators that one of them has started to inspect the reported product but hasn't yet resolved it, `resolved` tells moderators that one them has resolved the report. The value of `CreatedProductReportDefaultStatus` should be set to `active` as no one would have checked the report when it was created. Moderators can see all reports no matter their status and can also change the report status to any status they desire.

<br>

### **Register and login**

If you are not logged in you will have access only to the `Products`, `Product`, `Requests`, `Request` pages and you will not be able to buy products.
- You can log in as a common user by using one of the following seeded usernames and passwords in the login page [`alice`-`Password1!`, `bob`-`Password2!`, `charlie`-`Password3!`, `denis`-`Password4!`, `edward`-`Password5!`, `frank`-`Password6!`, `grace`-`Password7!`]. You can also create a new user by going to the register page and fill the required fields. Newly created users have no money in their balance and since their balance will always be below the products' prices the new users will not be able to buy products. In order to increase the user's balance you can create a product with a sell order, log out and log in as another user (who has money in his balance) and buy the product which will increase the balance of the seller and decrease the balance of the buyer. The app takes 10% from the product price as a tax which means the buyer pays the full product's price however the seller gets 90% of the product's price. An easier way to increase or decrease the balance of an existing user is to go to the `Balance` page and specify the amount of money.
- You can log in as a moderator by using one of the following seeded usernames and passwords in the login page [`moderator1`-`Password10!`,`moderator2`-`Password11!`,`moderator3`-`Password12!`]. You can also create a new moderator by registering a new user, login as an admin and change the role of the newly created user to `Moderator`. The only thing you can do as a moderator is to manage products and reports.
- You can log in as an admin by using one of the following seeded usernames and passwords in the login page [`admin1`-`Password8!`,`admin2`-`Password9!`]. The app doesn't provide a functionality to create new admins. As an admin you can manage products, reports and users. 

<br>
<br>
<br>
<br>
<br>

## **Regular user actions**

<br>

### **Create products**

In order to create products you have to go to `MyProducts` page and press the button `Create product` located at the top right corner. After that you will be redirected to `CreateProduct` page. You have to fill all text fields with valid data and you have to provide a .jpg files for the file fields. The only required file fields are the first one (which should be a picture showing the front side of the 3D model) and the last one (an image file which we will pretend to be a 3D model file). The front side picture is required because it will appear as a product picture in `MyProducts` and `Products` pages. The last image is needed because when users buy the product they need to be able to download the 3D model (in this case the picture that pretends to be the 3D model) they spent their money for. All other image files are optional but recommended to fill because they will show the way the 3D model looks from the other 5 out 6 main sides (the users will be able to see those pictures in `Product` page). When you fill all required fields with valid data and submit the form the app will create the product and will redirect you to the `Message` page (the app will not create the product if the user already has a product with the same name). The creation of the product happens in 2 steps - first the app saves the product text data in the database and after that the app will save the image files in `wwwroot/creators/currentUserUsername/ProductName`. Once you create the product you will be able to find it in `MyProducts` page and if you want to see some additional data about it you can click the button `View` for the product which will redirect you to `MyProduct`. Since the newly created product doesn't have any sell orders you will not be able to find it in `Products` page. 

<br>

### **Update product**

In order to update a product you have to go to `MyProducts` page and press the `edit` button for the product you want to edit. If the logged user has active sell order/s of the selected product the app will not allow you to edit the product and will redirect you to the `Message` page. If this happens you can either cancel all sell orders of the product or if the product has only one active sell order you can log out and log in as another user, buy the product and then log out and log in as the first user. If the selected product doesn't have any active sell orders after you press the `edit` button the app will redirect you to `UpdateProduct` page. The only required fields are the text fields which will be filled automatically with the product's data. For the file fields you should either provide .jpg files or provide nothing at all. The file fields which are left empty will not alter the existing image files while a file field for which you provide a file will override the existing one. When you fill all required fields with valid data and submit the form the app will apply your changes to the product and will redirect you to the `Message` page. You can check the changes in the pages `MyProducts` and `MyProduct`.

<br>

### **Create sell order**

In order to create a sell order you have to go to `MyProducts` choose a product and press the `View` button which will redirect you to the product's details. At the bottom of the page you should be able to see a button `Create sell order/s` and a text field before it. The app will not show those elements if the logged user has too many active sell orders nor if the product status is not `approved` but it will provide some text on their place explaining the reason you are not allowed to create sell orders of the product. If the app shows the button `Create sell order/s` and it's text field you can either enter a number in the field (which indicates the amount of sell orders you want to create) or leave it empty and press the button. The app will then redirect you to a confirmation page where you will be able to see the amount of sell orders you are about to create (the app will automatically lower their number if the number of active sell orders of the user combined with the number you entered exceeds the maximum number of active sell orders). Once you confirm the app will create the sell orders (their status will be `active`) and will redirect you to the `Message` page. You can find the current amount of active sell orders of the product in `MyProduct` and `Product` pages.

<br>

### **Cancel sell order**

Cancelling a sell order is very similar to creating one. The cancellation option is presented right above the creation one. The app will not show it if the product you selected has no active sell orders. If you see the cancellation option you do the same thing as for the creation option - you enter a number (or nothing) and then press the button `Cancel sell order/s`. The app then redirects you to a confirmation page and after you confirm, the app will cancel the specified amount of active sell orders of the product and will redirect you to the `Message` page. Cancelling an active sale order does not delete the order - it only set's the status of the order to `cancelled`.

<br>

### **Delete product**

In order to delete a product you have to go to `MyProducts` page and press the `delete` button for the product you want to delete. The app then redirects you to a confirmation page. After you confirm, the app will delete the product from the DB as well as its sell orders (completed orders are not touched).

<br>

### **Buy products**

In order to buy products you have to go to `Products` page and press the `Buy` button for the product you want to purchase. If the logged user doesn't have enough money or if he is the creator of the product or if he already purchased the product you will be redirected to the `Message` page where you will be able to see the reason the app is not allowing you to buy the product. Otherwise you will be redirected to a confirmation page. If you confirm the app will create a completed order record (which will be used for creating an invoice), it will set the status of the sell order to `completed` and will redirect you to the `Message` page. In order to download the 3D model file (in this case the picture that pretends to be the 3D model) you purchased you have to go to `Invoices` page, click the button `Details` (which will redirect you to `Invoice` page) for the desired invoice (since they are ordered in descending order by creation date the last order you created will be the first one to appear) and then press the button `Download`. In the `Invoice` page you will be able to see the seller's Username and if you log out and log in as the seller and go to the `Invoices` page you will find the same invoice in there, however the title will be a bit different and in the `Invoice` page you will see that the seller Username and download button are missing. The reason the download button is not presented in the seller's invoice page is because the seller is always the creator of the product which means the 3D model file (the picture that pretends to be the 3D model) was uploaded by him so he doesn't need to download it as he already has it.

<br>

### **Increase, decrease balance** ##

You can quickly increase or decrease the balance of a logged user by going to the `Balance` page. In order to increase the balance you have to enter a number in the text field `Increase balance` and press the button `Increase balance`. After you press the button the app will redirect you to the `Message` page showing the user's balance after the increment operation. Decreasing the balance works in the same way as increasing it (just use the text field and button `Decrease balance`).

<br>

### **Create order request**

In order to create an order request you have to go the `MyRequests` page and press the button `Create request` located at the top right corner. If you see a message which says "Max requests count reached!" instead of a button on the top left corner it means that the user has reached the maximum number of order requests. If this is the case you can delete an existing order request and the button should appear. Alternatively you can check whether any of the requests have a suggested product and if you find such product and purchase it the request will be marked as `completed` which should make the button `Create request` to appear. After you press the button `Create request` you will be redirected to `CreateOrderRequest` page. You have to fill all text fields with valid data - a title which should be the name of the desired product (for instance: chair, table, robot); a description which should provide additional information of what you want the 3D model to look like; a price which will tell sellers what is the maximum amount of money you plan to spend for the product. When you fill all required fields with valid data and submit the form the app will create the order request and will redirect you to the `Message` page (the app will not create the order request if the user already has an order request with the same title). Once you create the order request you will be able to find it in `MyRequests` page and if you want to see some additional data about it you can click the button `View` for the request which will redirect you to `MyRequest`.

<br>

### **Delete order request**

In order to delete an order request you have to go to `MyRequests` page and press the `Delete` button for the request you want to delete. The app then redirects you to a confirmation page. After you confirm, the app will set the status of the order request to `cancelled`.

<br>

### **Update order request**

In order to update an order request you have to go to `MyRequests` page and press the `edit` button for the request you want to edit. The app will then redirect you to the page `UpdateOrderRequest` where you can update the order request. When you fill all required fields with valid data and submit the form the app will apply your changes to the order request and will redirect you to the `Message` page. You can check the changes in the pages `MyRequests` and `MyRequest`.

<br>

### **Suggest a product to an order request**

In order to suggest a product to an order request you have to go `Requests` page and press the `View` button for a request you choose. After that the app will redirect to the page `Request` where you will be able to see the request description as well as a dropdown list containing names of your products and a `Suggest` button. If the logged in user doesn't have any active sell orders the app will show neither the dropdown list nor the `Suggest` button. In order to make the button to appear you will have to create at least one sell order of an existing product and if you don't have any products you will have to create at least one product before creating the sell order. After you press the `Suggest` button, the selected product from the dropdown list will be suggested to the request and you will be redirected to a message page. If you want to see the suggestion you can do the following: go to the `Request` page and see the user who created the request to which you suggested your product; log in as the user who created the order request and go to `MyRequest` page for the same request; in there you should be able to see a button `View suggestions`; after you press the button the app will redirect you to the `Products` page which will show only those products which were suggested to the order request; in there you should able to find the product you suggested with the first user.

<br>
<br>
<br>
<br>
<br>

## **Admin actions**

### **Change the role of a user**

In order to change the role of user you have to go the `Users` page and press the `Manage` button for the chosen user. If you don't see the button next to the user it means that the user is an admin and you cannot manage him at all. After you press the `Manage` button you will redirected to the page `ManageUser`. In there you can select a new role for the user and press the `Submit` button.

<br>

### **Suspend a user**

In order to suspend a user you have to go the `Users` page and press the `Manage` button for the chosen user. If you don't see the button next to the user it means that the user is an admin and you cannot manage him at all. After you press the `Manage` button you will redirected to the page `ManageUser`. In there you have to set the number of days to suspend the user and provide a lockout message. When the user tries to log in he will redirected to the lockout page where he will be able to see the reason he was locked out (this is lockout message you provided). In order to unsuspend a user you have to do the same thing and provide the value `0` where you specify "the days to suspend the user".

<br>

### **Change product status**

In order to change status of a product you have to go to the `Products` page and press the `Manage` button for the chosen product. After that the app will redirect you to the `ManageProduct` page where you can change the status of the product. The app will behave in the exact same way when the status of the product is `inspection` or `disapproved` but in production environment the status would tell other admins and moderators whether the product is worth to be checked (`inspection` means no one checked the product while `disapproved` means someone checked the product and did not approve it).

<br>

### **Change product report status**

In order to change status of a report you have to go to the `Reports` page and press the `Details` button for the chosen report. After that the app will redirect you to the `Report` page where you can change the status of the report. In this page you can check the report details and the details of the reported product before changing the status of the report.