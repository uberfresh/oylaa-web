<?php 
$user_id = $_POST['user_id'];

include 'database_connection.php';
$conn->set_charset("utf8");
$sql = "SELECT DISTINCT business_id,survey_id FROM tbl_answers where user_id = '$user_id' ORDER BY date DESC LIMIT 4 ";
$result = $conn->query($sql);
if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
     $a = $row['business_id'];
	
	
     $q2 = "SELECT * FROM tbl_business where business_id = '$a' ";
	 $r2 = $conn->query($q2);
	 if ($r2->num_rows > 0){
	 while ($row = $r2 -> fetch_assoc())
	 {
	   echo $row['business_name'].'~';
	 }
	 }
	
		
		
		
}

	
	
	}
else echo 0;
?>