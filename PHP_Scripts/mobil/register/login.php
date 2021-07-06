<?php

include '../database_connection.php';

if(isset($_POST['user_name'])&& isset($_POST['user_password']))
{

	$user_name=validate($_POST['user_name']);
	$user_password=validate($_POST['user_password']);
	

		$user_password=md5($user_password);
		$sqlquery="Select user_id from tbl_user where username='$user_name' and password='$user_password'";
	    $result=mysqli_query($conn,$sqlquery);
	
		if(mysqli_num_rows($result)===1)  
		{
			$row=mysqli_fetch_assoc($result);
			
		echo "1;".$row['user_id'];
		} 
	                                        
	    else echo "0";				
}
?>