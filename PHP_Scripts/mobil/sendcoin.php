
	<?php

include 'database_connection.php';


	$user_id=validate($_POST['user_id']);
	$target_id=validate($_POST['target_id']);
	$coin_amount=validate($_POST['coin_amount']);
/*
0:DB ERR!
1:OK!
2:TARGET_USER DOES NOT EXIST!
3:CANT SEND COIN TO YOURSELF
*/	if($target_id == $user_id)
	   echo "3";
      else {
        $checkUser = "SELECT user_id from tbl_user where user_id = $target_id";
        $result=mysqli_query($conn,$checkUser);
	    if(mysqli_num_rows($result) == 0)
			echo "2";
        else {
			  $sqlquery="INSERT INTO tbl_user_transaction(sender_id,target_id,coinamount,date_time) 
              values('$user_id','$target_id','$coin_amount',now())";
	          $result1=mysqli_query($conn,$sqlquery);
	          if ($result1)
		      echo "1";	
	          else echo "0";
			
              }
		  }
?>