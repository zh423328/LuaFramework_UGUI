LJ9   4   % > +   H  �PromptCtrl.New--->>logWarnx   4   % > 4    7  % +  7> G  �OnCreatePromptCreatePanelpanelMgrPromptCtrl.Awake--->>logWarn�  &,   7  , +  7% >, +  7% >, 4 % +  7$>+  74 7	+ 7
>4  7% 3 + 7>G  �����InitPanel  PromptItempromptLoadPrefabresMgrOnClickbtnOpenPromptPanelAddClick	nameStart lua--->>logWarnLuaBehaviourUIPanelGetComponenttransform�  3'd 4  7'  ' I+�4 8  >% 4	 
 >	$	:7	 7
 >74		 7	
	:	74		 7		:	+  	 7
 + 7>7	 7%
 >
 7	% >	4
  >
:
	K�G  ��	textGetComponent	TextFindChildOnItemClickAddClick	zerolocalPositiononeVector3localScaleSetParenttransformtostring	Item	namenewObjectgridParentPromptPanel    4  7 >G  	namelog�  &4  4 7 T�+  7>4  4 7 T�+  7>4  4 7 T�+  7>4  4 7 T�+  7	>4
 % 7 $>G  �	nameOnClick---->>>logWarnTestSendSprotoSPROTOTestSendPbcPBCTestSendPbluaPB_LUATestSendBinaryBINARYProtocalTypeTestProtoType�   ;+   7   % > 3 2 3 2 3 ;3 ;:''93 2 3 ;:' N9:	2 3
 2 3 ;:;:  7 %  >4 7> 74 7> 74 7> 7 >4  7 >G   �SendMessagenetworkMgrWriteBufferSPROTOProtocalTypeWriteByteMessageProtocalWriteShortNewByteBufferAddressBookencodeothers number9876543210 id��	name
Carolperson   	typenumber01234567890 id��	nameBob
phone 	typenumber87654321 	typenumber123456789 id�N	name
Alice�    .Person {
        name 0 : string
        id 1 : integer
        email 2 : string

        .PhoneNumber {
            number 0 : string
            type 1 : integer
        }

        phone 3 : *PhoneNumber
    }

    .AddressBook {
        person 0 : *Person(id)
        others 1 : *Person
    }
    
parse�  	  74   7  % $  4 7  % > 7% > 7>4	 7
 >3 2 3 ;3 ;:4	 7%  >4 7> 74 7> 74 7> 7 >4  7 >G  SendMessagenetworkMgrWriteBufferPBCProtocalTypeWriteByteMessageProtocalWriteShortNewByteBuffertutorial.Personencode
phone 	type	WORKnumber87654321 number1301234567 id�`	name
Aliceregisterprotobuf
close*a	readrb	openiolua/3rd/pbc/addressbook.pbDataPath	Util�    #4   7  > '�: % : % :   7 >4 7	> 7
4 7> 74 7> 7 >4  7 >G  SendMessagenetworkMgrWriteBufferPB_LUAProtocalTypeWriteByteMessageProtocalWriteShortNewByteBufferSerializeToStringjarjin@163.com
email	game	nameidLoginRequestlogin_pb�    4   7  >   7 4 7>  7 4 7>  7 %	 >  7
 '� >4  7  >G  SendMessagenetworkMgrWriteIntffff我的ffffQ靈uuuWriteStringBINARYProtocalTypeWriteByteMessageProtocalWriteShortNewByteBufferH    4     7  4 7> G  PromptCtrlNamesClosePanelpanelMgr� 
  94   % > 4   % > 4   % > 4   % > 4  % >4  % >2  5 4 * 4 1		 :	4 1	 :	
4 1	 :	4 1	 :	4 1	 :	4 1	 :	4 1	 :	4 1	 :	4 1	 :	4 1	 :	4 1	 :	0  �G   
Close TestSendBinary TestSendPblua TestSendPbc TestSendSproto OnClick OnItemClick InitPanel OnCreate 
Awake NewPromptCtrl3rd/sproto/print_rsproto.core3rd/sproto/sproto3rd/pbc/protobuf3rd/pblua/login_pbCommon/definerequire 