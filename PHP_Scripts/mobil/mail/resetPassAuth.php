<?php
include "../database_connection.php";
include 'class.phpmailer.php';



	$user_name=validate($_POST['user_name']);	
	$authCode=validate($_POST['authCode']);

/*
 * 0:Auth can not send.
 * 1:Ok!
 * 2:Username not valid 

 */
		
		$mailCheckQuery="Select email from tbl_user where  username='$user_name'";
		$result=mysqli_query($conn,$mailCheckQuery);
		if(mysqli_num_rows($result)==0)
		{	
			echo "2";
			exit();
		}
		else{
		$row=mysqli_fetch_assoc($result);
		$user_mail = $row["email"];	
	    $mail = new PHPMailer();
		$mail->IsSMTP();
		$mail->SMTPAuth = true;
		$mail->Host = 'oylaa.online';
		$mail->Port =587;
		$mail->SMTPSecure = 'tls';
		$mail->Username = 'info@oylaa.online';
	    $mail->Password = 'imyo_soft2021.';
		$mail->SetFrom($mail->Username, 'Oylaa Destek');
		$mail->AddAddress($user_mail, $user_name);
		$mail->CharSet = 'UTF-8';
		$mail->Subject = 'E-Posta Onay';
		
		$content = '<div style="background: #eee; padding: 10px; font-size: 14px"> Sayın '.$user_name.' Oylaa! uygulaması şifre yenileme doğrulama kodunuz : <strong>'.$authCode.'</strong></div>';
		$mail->MsgHTML($content);
		if($mail->Send()) 
			echo "1";                   			                 
		else echo "0";
		
			}

?>