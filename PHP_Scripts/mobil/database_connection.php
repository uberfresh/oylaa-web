<?php
$host="localhost";
$uname="oylaa";
$password="oylaa1234";
$db_name="oylaakee";
$conn=mysqli_connect($host,$uname,$password,$db_name);
mysqli_set_charset($conn,"utf-8");
$conn->set_charset("utf8");
function validate($data)
	{
		$data=trim($data);
		$data=stripslashes($data);
		$data=htmlspecialchars($data);
		return $data;
	}

if(!$conn)
{
	echo "0";
}

	

?>