<?php 
$business_id = $_POST['business_id']; 

include '../database_connection.php';
$conn->set_charset("utf8");
$sql = "SELECT *  FROM tbl_campaign  where business_id = $business_id  ORDER BY date DESC";
$result = $conn->query($sql);
if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
     $b_id = $row['business_id'];
	 echo $row['campaign_name'].'~'.$row['campaign_content'].'~'.$row['campaign_coin'].'~'.$row['id'].'#';		
}
}
    $q2 = "SELECT DISTINCT business_name  FROM tbl_business where business_id = '$b_id' ";
	 $r2 = $conn->query($q2);
	 if ($r2->num_rows > 0){
	 while ($row = $r2 -> fetch_assoc())
	 {
	   echo $row['business_name'];
	 }
	 }

else echo 0;