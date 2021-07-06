<?php 
$user_id =$_POST['user_id'];

include 'database_connection.php';
$conn->set_charset("utf8");
$sql = "SELECT * FROM tbl_answers where question_type = 2 and user_id=$user_id order by date desc LIMIT 1";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
  // output data of each row
  while($row = $result->fetch_assoc()) {
	  $id = $row['business_id'];
  echo $row['chosen_answer'].'~'.$row['date'].'~';
  
  }



}
$sql2 = "SELECT DISTINCT business_name FROM tbl_business where business_id = $id";
$result2 = $conn->query($sql2);

if ($result2->num_rows > 0) {

  while($row2 = $result2->fetch_assoc()) {
         
   echo $row2['business_name'];  
  }}

?>