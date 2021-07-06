<?php
include "../database_connection.php";

	$user_pass=validate($_POST['user_pass']);	
	$user_name = validate($_POST['user_name']);
/*
 * 0:Database error.
 * 1:Ok!
 * 
 */
         $user_pass = md5($user_pass);
		
		$query="UPDATE tbl_user SET password = '$user_pass' where  username='$user_name'";
		if(mysqli_query($conn,$query))
			echo "1";
         else "0";
		
?>