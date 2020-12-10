// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class QnABot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
    {
        protected readonly BotState ConversationState;
        protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;
        protected readonly BotState UserState;

        public QnABot(ConversationState conversationState, UserState userState, T dialog)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken) =>
            // Run the Dialog with the new message Activity.
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);

                    await SendSuggestedActionsAsync(turnContext, cancellationToken, 0);
                }
            }
        }
        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken, int function) //��ư
        {

            switch (function)
            {

                case 0://���̿��� ����� ��ȭ�� ������ ��=> �� ó���� �����
                       ////
                       ////
                       //�̹���
                    var image = ProcessInput(turnContext);

                    image.Text = "'�����ٺ�'�� �߰����ּż� �����մϴ�.";
                    image.Attachments = new List<Attachment>() { GetInternetAttachment() };
                    await turnContext.SendActivityAsync(image, cancellationToken);



                    var welcomeText = turnContext.Activity.CreateReply("���� �� ���̴� ���� ��, �ñ����� �����Ű���? \r\n ���� �˷��帱�Կ�!\r\n");
                    welcomeText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "�λ��ϱ�", Type = ActionTypes.ImBack, Value = "�ȳ�" },
                        new CardAction() { Title = "�������ϱ�", Type = ActionTypes.ImBack, Value = "���� �˷��ּ���" },
                        new CardAction() { Title = "��� �˷��ֱ�", Type = ActionTypes.ImBack, Value = "����" },
                        //new CardAction() { Title = "�˻������� ���Ӹ�", Type = ActionTypes.ImBack, Value = "���" },
                    },
                    };
                    await turnContext.SendActivityAsync(welcomeText, cancellationToken);
                    ////
                    ////
                    break;

                case 1://���� => ���� ��ư�� �����ų� "����"�̶�� ä���� ���� �����
                    var binText = turnContext.Activity.CreateReply("ä��â�� \"�ȳ�\", \"���� �˷��ּ���\", \"����\", \"���\"�� �Է��غ�����!");
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

        //�̹����� ������ ���ؼ� ������ Activity ��ü�� ��������� �۾�
        private static Activity ProcessInput(ITurnContext turnContext) //�̹����� ������ ���ؼ� ������ Activity ��ü�� ��������� �۾�
        {
            var activity = turnContext.Activity;
            var reply = activity.CreateReply();

            return reply;
        }
        //�̹��� ���̱�
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
