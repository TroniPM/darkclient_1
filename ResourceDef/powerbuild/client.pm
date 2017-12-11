#!/usr/bin/perl -w
use IO::Socket;
use IO::Select;
package client;
my $taskstodo;
my $tasksonworking;
my $tasksfinished;
my $taskscollected;

sub new
{
	my $this={};
	bless $this;
	return $this;
}

sub setsocket
{
	my ($this,$socketref)=@_;
	$this->{"socket"}=$socketref;
	binmode(${$socketref});
}

sub setsel
{
	my($this,$selref)=@_;
	$this->{"sel"}=$selref;
}

sub settaskstodo
{
	$taskstodo=$_[0];
}

sub settasksfinished
{
	$tasksfinished=$_[0];
}

sub settasksonworking
{
	$tasksonworking=$_[0];
}

sub settaskscollected
{
	$taskscollected=$_[0];
}

sub test
{
	print ${$taskstodo}{"x"} . "\n";
}

sub printtest
{
	my ($this)=@_;
	print ${$this->{"test"}} . "\n";
}

sub Process
{
	my($this)=@_;
	#do somthing to deal with the socket

	${$this->{"socket"}}->recv($_,1024,0);
	#my $fh=${$this->{"socket"}};
	#$_=<$fh>;
	if(not $_ )
	{
		return "close";
	}
	else
	{
		@temp=unpack("ccs",substr($_,0,4));
		my $packhead;
		my $ret="default\n";
		print " cmd is $temp[0] package length ". (length)."\n";
		if($temp[0] eq "0")
		{
			#close socket
			return "close";
		}
		elsif($temp[0] eq "1")
		{
			#try to get task
			if(%{$taskstodo})
			{
				my @k=keys %{$taskstodo};
				$ret=$k[0];
				${$tasksonworking}{$ret}=1;
				delete ${$taskstodo}{$ret};
			}
			else
			{
				$ret="upload your work\n";
			}
			$packhead=pack("ccs",1,1,length($ret));
		}
		elsif($temp[0] eq "2")
		{
			#try to upload a file
			print "type is " . $temp[1]." data len is ". $temp[2] . "\n";
			if($temp[1] == '1')
			{
				#create a file on server
				my $filename=substr($_,4,$temp[2]);
				print $filename;
				chomp($filename);
				if(-e "mogo/".$filename)
				{
					$ret="already exist\n";
				}
				else
				{
					my $F=CreateFile($filename);
					if($F)
					{
						$this->{"fileref"}=$F;
						$ret="sucess\n";	
					}
					else
					{
						$ret="fail\n";
					}
				}
				$packhead=pack("ccs",2,1,length($ret));
			}
			elsif($temp[1] == '2')
			{
				#write binary to file
				my $buffer=substr($_,4,$temp[2]-1);
				if($this->{"fileref"})
				{
					print " buffer size " . length($buffer) . "\n";
					my $F=${$this->{"fileref"}};
					binmode $F;
					print $F $buffer;
					$ret="sucess\n";
				}
				else
				{
					$ret="fail\n";
				}
				$packhead=pack("ccs",2,1,length($ret));
			}
			elsif($temp[1] == '3')
			{
				#the last part of a file
				if(exists $this->{"fileref"})
				{
					@st=stat(${$this->{"fileref"}});
					print "file size is " .$st[7] ."\n";
					if(${$this->{"fileref"}})
					{
						close ${$this->{"fileref"}};
						delete $this->{"fileref"};
					}
				}
				$ret="sucess\n";
				$packhead=pack("ccs",2,1,length($ret));
			}
		}
		elsif($temp[0] eq "3")
		{
			#upload tasks
			$task=substr($_,4,$temp[2]);
			${$taskstodo}{$task}=1;
			print "new add task ---> $task";
			$ret="sucess\n";
			$packhead=pack("ccs",3,1,length($ret));
		}
		elsif($temp[0] eq "4")
		{
			my @e;
			my $count;
			if($taskstodo)
			{
				@e=keys %{${$taskstodo}};
				$count=@e;
				$ret="total task count -> " .$count ;
			}
			$packhead=pack("ccs",4,1,length($ret));
		}
		print "return is $ret \n";
		${$this->{"socket"}}->send($packhead.$ret,0);
		${$this->{"socket"}}->flush;
		$_="";
	}
}
sub CreateFile
{
	my ($filename)=@_;
	$filename=~s/\#/\\\#/g;
	$filename=~s/\s/\\ /g;
	print "file name is " . $filename ."\n";
	my @temp=split(/\//,$filename);
	my $count=@temp;
	my $dir="mogo/";
	my $path="mogo/".$_[0];
	for($i=0;$i<($count -1 );$i++)
	{
		$dir=$dir . $temp[$i] . "/";
	}
	if(not -e $dir)
	{
		system("mkdir -p ".$dir);
	}
	#system("touch $path");

	my $F;
	print $path . "\n";
	open($F,">$path") or print "open file fail\n";
	return \$F;
}

1;
