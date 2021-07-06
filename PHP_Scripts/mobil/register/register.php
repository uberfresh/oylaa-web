<?php
include "../database_connection.php";
if(isset($_POST['user_name'])&& isset($_POST['user_mail'])&& isset($_POST['user_password'])&& isset($_POST['user_age']) && isset($_POST['user_gender']))
{


	$user_name=validate($_POST['user_name']);	
	$user_mail=validate($_POST['user_mail']);
    $user_password=validate($_POST['user_password']);
	$user_age=validate($_POST['user_age']);
    $user_gender=validate($_POST['user_gender']);
	
/*
 * 0:Database error
 * 1:Ok!
 * 2:Mail already in use
 * 3:Mail not valid
 * 4:Username already in use
 */
		
		      $user_password=md5($user_password);
	          $insert="Insert Into tbl_user(username,email,password,balance,gender,age,date_time)                                       values('$user_name','$user_mail','$user_password','0','$user_gender','$user_age',now())";
		      $result=mysqli_query($conn,$insert);
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
} 	
?>