<?php

include 'database_connection.php';

if(isset($_POST['user_id']))
{

	$user_id=validate($_POST['user_id']);
	$sqlquery="Select balance from tbl_user where user_id='$user_id'";
	$result=mysqli_query($conn,$sqlquery);
	
		if(mysqli_num_rows($result)>0)  
		{
		  $row=mysqli_fetch_assoc($result);
		  echo $row['balance'];
		} 	                                        
	    else echo "err".$user_id;				
}
?>