
	<?php

include 'database_connection.php';

	$user_id=validate($_POST['user_id']);
	$sqlquery="Select username from tbl_user where user_id='$user_id'";
	$result=mysqli_query($conn,$sqlquery);
	
		if(mysqli_num_rows($result)>0)  
		{
		  $row=mysqli_fetch_assoc($result);
		  echo $row['username'];
		} 	                                        
	    else echo "err".$user_id;				

?>
	


