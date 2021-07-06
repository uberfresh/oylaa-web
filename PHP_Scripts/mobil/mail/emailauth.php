<?php
include "../database_connection.php";
include 'class.phpmailer.php';



	$user_name=validate($_POST['user_name']);	
	$user_mail=validate($_POST['user_mail']);
	$authCode=validate($_POST['authCode']);

/*
 * 0:Database error
 * 1:Ok!
 * 2:Mail already in use
 * 3:Mail not valid
 * 4:Username already in use
 * 5:Auth can not send.
 */
		
		$mailCheckQuery="Select * from tbl_user where  email='$user_mail'";
		$result=mysqli_query($conn,$mailCheckQuery);
		if(mysqli_num_rows($result)>0)
		{	
			echo "2";
			exit();
		}
		else{
			if (filter_var($email, FILTER_VALIDATE_EMAIL))
			{
			 echo "3";
		     exit();
			}  
			else {
	         $userCheckQuery = "Select * from tbl_user where username='$user_name'"; 
		     $result = mysqli_query($conn,$userCheckQuery);
		    
			 if(mysqli_num_rows($result)>0)
		     {
		      echo "4";
		      exit();
		     }
			 
		     else {
				 
				 
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
		
		$content = '<div style="background: #eee; padding: 10px; font-size: 14px"> Sayın '.$user_name.' Oylaa! uygulamasına üye olmak için  doğrulama kodunuz : <strong>'.$authCode.'</strong></div>';
		$mail->MsgHTML($content);
		if($mail->Send()) 
			echo "1";                   			                 
		else echo "0";
	 			 			 
			    }
		          
			}
			}

?>