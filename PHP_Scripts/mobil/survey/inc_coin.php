
<?php 

include '../database_connection.php';
$conn->set_charset("utf8");

$user_id = $_POST["user_id"];


$sql = "UPDATE tbl_user SET balance = balance+150 where user_id = $user_id ";
$result = $conn->query($sql);

if($result)
{
    echo "1";
}
else echo "0";


?>