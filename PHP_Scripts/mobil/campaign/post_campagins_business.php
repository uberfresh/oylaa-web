<?php 
$business_id = $_POST['business_id']; 
$campaign_id=$_POST['campaign_id'];
$user_id = $_POST['user_id'];
$coin_amount = $_POST['coin'];
include '../database_connection.php';
$conn->set_charset("utf8");
$sql = "INSERT INTO tbl_business_transaction(business_id,campaign_id,user_id,coinamount) values ('$business_id','$campaign_id','$user_id','$coin_amount')" ;
  $result=mysqli_query($conn,$sql);
		          if($result)
		         {
			       echo "1";
			       exit();
		         }
		          else
		         {
			      echo "0";
			      exit();
		         }
  

