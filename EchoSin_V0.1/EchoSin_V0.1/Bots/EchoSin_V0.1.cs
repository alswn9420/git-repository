
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace EchoSin_V0._1.Bots
{
    public class EchoBot : ActivityHandler
    {
        private Dictionary<string,string> dataSet = new Dictionary<string, string>();
        //if(GetDataFromServer());
         //�޽��� �۾��� ���ŵ�: �޽��� �۾��� ó���ϵ��� ��������
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            
            var Text = turnContext.Activity.Text;
            //�޽����� �����ϱ�����
            var replyText = $"���� �������� ���� �κ��Դϴ�. \r\n  �� �𸣰ڴٸ� \"����\"�� �Է��ϼ���. \r\n";
            if (!GetDataFromServer())
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("��"), cancellationToken);
            }
            //Reply��ü���� �ش� reply ��������          
            //�ش� reply�� ���� ��� -> echo
            //�ش� reply�� �ִ� ��� -> reply
            if (Text.Equals("�ȳ�"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("�ȳ��ϼ���, '�����ٺ�'�� ��ȹ�� �ż��� �� �Դϴ�.\r\n �ż����� ������� �������� ��ģ����, ���� ���ϴ� ��������� �ս��� ã�� �� �ִٴ� �ǹ��Դϴ�. \r\n"), cancellationToken);
            }
            else if (Text.Equals("���� �˷��ּ���"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("����� ����� ����� ����鿡�� ����� �˷��ְ��� \r\n ������ �˸��� ê���� ����� �ֽ��ϴ�."), cancellationToken);
            }
            else if (Text.Equals("����"))
            {
                await SendSuggestedActionsAsync(turnContext, cancellationToken, 1);
            }
            else if(Text.Equals("���"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("������\r\n���׾�\r\n������\r\n���߽�\r\nJMT\r\n������\r\n����\r\n�� 7���� ����� �˻��� �� �ֽ��ϴ�.\r\n ���ϴ� �������� �ܾ �Է��ϼ���"), cancellationToken);
            }
            else 
            {
                string data = SearchData(Text);
                if (data != null)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(data, data), cancellationToken);
                }
                else
                {
                    //�޽����� �۽��ϱ����� turnContext ��ü�� SendActivityAsync�޼ҵ� ���
                    //���� �������� ���� �κ��Դϴ�. ���
                    await turnContext.SendActivityAsync(MessageFactory.Text(replyText+"�˻��� �����Ͱ� �������� �ʽ��ϴ�", replyText), cancellationToken);
                }

            }

        }
        protected bool GetDataFromServer()
        {
            WebRequest request = WebRequest.Create("word.txt");
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            if(responseFromServer != null)
            {
                string Datakey = null, Datavalue = null;
                string[] splitData = responseFromServer.Split('@');
                for(int i = 0; i<splitData.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        Datakey = splitData[i];
                        continue;
                    }
                    else
                    {
                        Datavalue = splitData[i];
                        dataSet.Add(Datakey, Datavalue);
                    }
                }
                return true;
            }
            return false;
        }
        protected string SearchData(string key)
        {
            if (dataSet.ContainsKey(key))
            {
                return dataSet[key];
            }
            return null;
        }
        //���̿��� ����� ��ȭ�� ������ : ��ȭ�� ������ ����� ó���ϵ��� ��������
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //await turnContext.SendActivityAsync(welcomeText, cancellationToken);
                    //await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                    await SendSuggestedActionsAsync(turnContext, cancellationToken, 0);
                }

            }
        }

        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken, int function) //��ư
        {

            switch (function)
            {

                case 0://���̿��� ����� ��ȭ�� ������ ��=> �� ó���� �����

                    //�̹���
                    var image = ProcessInput(turnContext);

                    image.Text = "'�����ٺ�'�� �߰����ּż� �����մϴ�.";
                    image.Attachments = new List<Attachment>(){ GetInternetAttachment() };
                    await turnContext.SendActivityAsync(image, cancellationToken);



                    var welcomeText = turnContext.Activity.CreateReply("���� �� ���̴� ���� ��, �ñ����� �����Ű���? \r\n ���� �˷��帱�Կ�!");
                    welcomeText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "�λ��ϱ�", Type = ActionTypes.ImBack, Value = "�ȳ�" },
                        new CardAction() { Title = "�������ϱ�", Type = ActionTypes.ImBack, Value = "���� �˷��ּ���" },
                        new CardAction() { Title = "��� �˷��ֱ�", Type = ActionTypes.ImBack, Value = "����" },
                        new CardAction() { Title = "�˻������� ���Ӹ�", Type = ActionTypes.ImBack, Value = "���" },
                    },
                    };
                    await turnContext.SendActivityAsync(welcomeText, cancellationToken);
                    break;

                case 1://���� => ���� ��ư�� �����ų� "����"�̶�� ä���� ���� �����
                    var binText = turnContext.Activity.CreateReply("ä��â�� \"�ȳ�\", \"���� �˷��ּ���\", \"����\", \"���\"�� �Է��غ�����!");
                    binText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "�λ��ϱ�", Type = ActionTypes.ImBack, Value = "�ȳ�" },
                        new CardAction() { Title = "�������ϱ�", Type = ActionTypes.ImBack, Value = "���� �˷��ּ���" },
                        new CardAction() { Title = "��� �˷��ֱ�", Type = ActionTypes.ImBack, Value = "����" },
                        new CardAction() { Title = "�˻������� ���Ӹ�", Type = ActionTypes.ImBack, Value = "���" },
                    },
                    };
                    await turnContext.SendActivityAsync(binText, cancellationToken);
                    break;

            }

        }
        private static Activity ProcessInput(ITurnContext turnContext) //�̹����� ������ ���ؼ� ������ Activity ��ü�� ��������� �۾�
        {
            var activity = turnContext.Activity;
            var reply = activity.CreateReply();

            return reply;
        }
        private static Attachment GetInternetAttachment() //
        {
            // ContentUrl must be HTTPS.
            return new Attachment
            {
                Name = @"2iAKkh.jpgresize.png",
                ContentType = "image/png",
                ContentUrl = "https://ifh.cc/g/2iAKkh.jpgresize.png",
            };
        }


    }

}
