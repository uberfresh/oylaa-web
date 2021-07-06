
<?php 

include '../database_connection.php';
$conn->set_charset("utf8");
$business_id =$_POST["bussines_id"];
$survey_id =$_POST["survey_id"];
$user_id = $_POST["user_id"];
$chosen_answer = $_POST["chosen_answer"];
$question_type = $_POST['q_types'];
$question_id = $_POST['q_id'];

$sql = "INSERT INTO tbl_answers(business_id,survey_id,user_id,question_id,question_type,chosen_answer) VALUES ('$business_id','$survey_id','$user_id','$question_id','$question_type','$chosen_answer')";


$result = $conn->query($sql);

if($result)
{
    echo "1";
}
else echo "0";


?>