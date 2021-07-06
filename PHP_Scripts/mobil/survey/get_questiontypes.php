<?php 

include '../database_connection.php';
$bussines_id = $_POST["bussines_id"];
$survey_id =$_POST["survey_id"];

$conn->set_charset("utf8");
$sql = "SELECT * FROM tbl_surveys where survey_id='$survey_id' and business_id= '$bussines_id'";
$result = $conn->query($sql);
if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
     
     echo $row['questiontype'];

}
	
}else echo 1;






?>