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
        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken, int function) //버튼
        {

            switch (function)
            {

                case 0://봇이외의 멤버가 대화에 참가함 ㄴ=> 맨 처음에 띄워짐
                    ////
                     ////
                    //이미지
                    var image = ProcessInput(turnContext);

                    image.Text = "'별다줄봇'을 추가해주셔서 감사합니다.";
                    image.Attachments = new List<Attachment>(){ GetInternetAttachment() };
                    await turnContext.SendActivityAsync(image, cancellationToken);



                    var welcomeText = turnContext.Activity.CreateReply("별걸 다 줄이는 요즘 말, 궁금하지 않으신가요? \r\n 저희가 알려드릴게요!\r\n 현재 검색 가능한 신조어는 별다줄,얼죽아,얼죽코,졌잘싸,JMT,존맛탱,핑프가 있습니다.");
                    welcomeText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "인사하기", Type = ActionTypes.ImBack, Value = "안녕" },
                        new CardAction() { Title = "목적말하기", Type = ActionTypes.ImBack, Value = "목적 알려주세요" },
                        new CardAction() { Title = "기능 알려주기", Type = ActionTypes.ImBack, Value = "도움말" },
                        //new CardAction() { Title = "검색가능한 줄임말", Type = ActionTypes.ImBack, Value = "목록" },
                    },
                    };
                    await turnContext.SendActivityAsync(welcomeText, cancellationToken);
                    ////
                    ////
                    break;

                case 1://도움말 => 도움말 버튼을 누르거나 "도움말"이라고 채팅이 오면 띄워짐
                    var binText = turnContext.Activity.CreateReply("채팅창에 \"안녕\", \"목적 알려주세요\", \"도움말\", \"목록\"을 입력해보세요!");
                    binText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "인사하기", Type = ActionTypes.ImBack, Value = "안녕" },
                        new CardAction() { Title = "목적말하기", Type = ActionTypes.ImBack, Value = "목적 알려주세요" },
                        new CardAction() { Title = "기능 알려주기", Type = ActionTypes.ImBack, Value = "도움말" },
                        //new CardAction() { Title = "검색가능한 줄임말", Type = ActionTypes.ImBack, Value = "목록" },
                    },
                    };
                    await turnContext.SendActivityAsync(binText, cancellationToken);
                    break;

            }

        }
        
        //이미지를 보내기 위해서 별도의 Activity 객체를 만들기위한 작업
        private static Activity ProcessInput(ITurnContext turnContext) //이미지를 보내기 위해서 별도의 Activity 객체를 만들기위한 작업
        {
            var activity = turnContext.Activity;
            var reply = activity.CreateReply();

            return reply;
        }
        //이미지 붙이기
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
