#!/usr/bin/perl -w
use IO::Socket;
use IO::Select;
$cmdEXEMethod="/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod ";
$argc=@ARGV;
if($argc==1)
{
	OneArg($ARGV[0]);
}
elsif($argc>=2)
{
	TwoArg(@ARGV);
}

sub OneArg
{
	$_=shift;
	print "inside one arg " . $_ . "\n";
	if($_ eq  "-ba")
	{
		BuildAllAssetBundle();
	}
	elsif($_ eq "-bae")
	{
		BuildAllAssetBundleEx();
	}
	elsif($_  eq "-el")
	{
		EditorLog();
	}
	elsif($_ eq "-pb")
	{
		PowerBuild();
	}
	elsif($_ eq "-p")
	{
		QueryProgress();
	}
	else
	{
		print "unknown param \n";
	}
}

sub TwoArg
{
	if($_[0] eq "-e")
	{
		ExeMethod($_[1]);
	}
	elsif($_[0] eq "-ts")
	{
		TrimStart($_[1],$_[2],$_[3]);
	}
	elsif($_[0] eq "-df")
	{
		Dif($_[1],$_[2],$_[3]);
	}
}

sub Dif
{
	open(S,"<$_[0]") or die"$@";
	open(S1,"<$_[1]") or die"$@";
	system("touch $_[2]");
	open(D,">$_[2]");
	my %f=();
	my %f2=();
	while(<S>)
	{
		$f{$_}=1;
	}
	while(<S1>)
	{
		$f1{$_}=1;
	}
	while(my ($k,$v)=each %f)
	{
		if(not exists $f1{$k})
		{
			print D $k;
		}
	}
	close S;
	close S1;
	close D;
}

sub TrimStart
{
	open(S,"<$_[0]");
	system("touch $_[1]");
	open(D,">$_[1]");
	$start=$_[2];
	while(<S>)
	{
		s/$start//g;
		if(/\.DS/)
		{
			next;
		}
		else
		{
			print D ;
		}
	}
	close S;
	close D;
	print "trim start sucess remove start --> ".$start . "\n";
}

sub Test
{
	
}

sub EditorLog
{
	open(F,"</Users/".$ENV{"USER"}."/Library/Logs/Unity/Editor.log");
	while(1)
	{
		if($line=<F>)
		{
			print $line;
		}
		else
		{
			sleep(1);#in perl this means sleep for 1 second
		}
	}
}

sub ExeMethod
{
	print ($cmdEXEMethod . $_[0] . "\n");
#	system(cmdEXEMethod . @_[0]);
}

sub BuildAllAssetBundle
{
	print "inside build all asset bundle\n";
	my $F;
	open($F,"<AllResources.txt") or die"cannot open file ! \n";
	my $ser_addr = "172.16.10.42";
	my $ser_port = "9527";
	my $socket = IO::Socket::INET->new(
			PeerAddr=>"$ser_addr",
			PeerPort=>"$ser_port",
			Type=>SOCK_STREAM,
			Proto=>"tcp",) or die "cannot create socket .$@";
	$socket->autoflush(1);
	while(<$F>)
	{
		chomp;
		$socket->send("+->$_\n",0);
		my $ret=<$socket>;
		print $ret . " for " . $_ . "\n";	
	}
	$socket->send("close\n",0);
	$socket->close();
	close($F);
}

sub BuildAllAssetBundleEx
{
	my $F;
	open($F,"<AllResources.txt") or die"cannot open file ! \n";
	my $ser_addr = "172.16.10.42";
	my $ser_port = "9528";
	my $socket = IO::Socket::INET->new(
			PeerAddr=>"$ser_addr",
			PeerPort=>"$ser_port",
			Type=>SOCK_STREAM,
			Proto=>"tcp",) or die "cannot create socket .$@";
	$socket->autoflush(1);
	my $packhead;
	while(<$F>)
	{
		$packhead=pack("CCs",3,0,length);
		$socket->send($packhead.$_,0);
		my $ret=<$socket>;
	}
	$packhead=pack("CCs",0,0,0);
	my $ret=$packhead . "\n";
	$socket->send($ret,0);
	$socket->close();
	close($F);
}
sub QueryProgress
{
	my $ser_addr = "172.16.10.42";
	#my $ser_addr="127.0.0.1";
	my $ser_port = "9527";
	my $socket = IO::Socket::INET->new(
				PeerAddr=>"$ser_addr",
				PeerPort=>"$ser_port",
			    Type=>SOCK_STREAM,
				Proto=>"tcp",) or die "cannot create socket .$@";
	$socket->autoflush(1);
	$socket->send("?->?\n",0);
	my $ret=<$socket>;
	print $ret;
	$socket->send("close\n",0);
	$socket->close;
}

sub PowerBuild
{
	$cmd=$cmdExeMethod . " OSXTools.CollectAllResources";
	system($cmd);
	$cmd="mg -bae";
	system($cmd);
}
