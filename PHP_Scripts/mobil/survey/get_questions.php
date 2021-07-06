<?php 
$business_id =$_POST["bussines_id"];
$survey_id =$_POST["survey_id"];

include '../database_connection.php';

$sql = "SELECT * FROM tbl_surveys where survey_id='$survey_id' and business_id= '$business_id'";
$result = $conn->query($sql);
if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
     
     echo $row['question'].'~'.$row['id'].'~';

}
}







?>