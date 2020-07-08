// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace EchoSin_V0._1.Bots
{
    public class EchoBot : ActivityHandler
    {

        //�޽��� �۾��� ���ŵ�: �޽��� �۾��� ó���ϵ��� ��������
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var Text = turnContext.Activity.Text;
            //�޽����� �����ϱ�����
            var replyText = $"���� �������� ���� �κ��Դϴ�.";

            //Reply��ü���� �ش� reply ��������          
            //�ش� reply�� ���� ��� -> echo
            //�ش� reply�� �ִ� ��� -> reply
            if (Text.Equals("�ȳ�"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("�ȳ��ϼ���, �ż��� �� �Դϴ�.\r\n �ż����� ������� �������� ��ģ����, ���� ���ϴ� ��������� �ս��� ã�� �� �ִٴ� �ǹ��Դϴ�. "), cancellationToken);
            }
            else if (Text.Equals("���� �˷��ּ���"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("�츮�� ����� ����� ����鿡�� ����� �˷��ְ��� \r\n ������ �˸��� ê���� ����� �ֽ��ϴ�."), cancellationToken);
            }
            else if (Text.Equals("����"))
            {
                await SendSuggestedActionsAsync(turnContext, cancellationToken, 1);
            }
            else
            {
                //�޽����� �۽��ϱ����� turnContext ��ü�� SendActivityAsync�޼ҵ� ���
                //���ڱ��
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }

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

        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken, int function)
        {


            switch (function)
            {

                case 0://���̿��� ����� ��ȭ�� ������

                    var welcomeText = turnContext.Activity.CreateReply("Hello and welcome!");
                    welcomeText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "�λ��ϱ�", Type = ActionTypes.ImBack, Value = "�ȳ�" },
                        new CardAction() { Title = "�������ϱ�", Type = ActionTypes.ImBack, Value = "���� �˷��ּ���" },
                        new CardAction() { Title = "��� �˷��ֱ�", Type = ActionTypes.ImBack, Value = "����" },
                    },
                    };
                    await turnContext.SendActivityAsync(welcomeText, cancellationToken);
                    break;

                case 1://����
                    var binText = turnContext.Activity.CreateReply("ä��â�� \"�ȳ�\", \"���� �˷��ּ���\", \"����\"�� �Է��غ�����!");
                    binText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "�λ��ϱ�", Type = ActionTypes.ImBack, Value = "�ȳ�" },
                        new CardAction() { Title = "�������ϱ�", Type = ActionTypes.ImBack, Value = "���� �˷��ּ���" },
                        new CardAction() { Title = "��� �˷��ֱ�", Type = ActionTypes.ImBack, Value = "����" },
                    },
                    };
                    await turnContext.SendActivityAsync(binText, cancellationToken);
                    break;

            }

        }

    }

}
