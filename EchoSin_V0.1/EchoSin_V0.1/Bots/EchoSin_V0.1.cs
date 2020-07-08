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

        //메시지 작업이 수신됨: 메시지 작엄을 처리하도록 재정의함
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var Text = turnContext.Activity.Text;
            //메시지를 수신하기위해
            var replyText = $"아직 구현되지 않은 부분입니다.";

            //Reply객체에서 해당 reply 가져오기          
            //해당 reply가 없는 경우 -> echo
            //해당 reply가 있는 경우 -> reply
            if (Text.Equals("안녕"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("안녕하세요, 신세권 팀 입니다.\r\n 신세권은 신조어와 역세권을 합친말로, 흔히 접하는 신조어들을 손쉽게 찾을 수 있다는 의미입니다. "), cancellationToken);
            }
            else if (Text.Equals("목적 알려주세요"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("우리는 신조어가 어색한 사람들에게 신조어를 알려주고자 \r\n 신조어 알리미 챗봇을 만들고 있습니다."), cancellationToken);
            }
            else if (Text.Equals("도움말"))
            {
                await SendSuggestedActionsAsync(turnContext, cancellationToken, 1);
            }
            else
            {
                //메시지를 송신하기위해 turnContext 객체의 SendActivityAsync메소드 사용
                //에코기능
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }

        }

        //봇이외의 멤버가 대화에 참가함 : 대화에 참가한 멤버를 처리하도록 재저의함
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

                case 0://봇이외의 멤버가 대화에 참가함

                    var welcomeText = turnContext.Activity.CreateReply("Hello and welcome!");
                    welcomeText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "인사하기", Type = ActionTypes.ImBack, Value = "안녕" },
                        new CardAction() { Title = "목적말하기", Type = ActionTypes.ImBack, Value = "목적 알려주세요" },
                        new CardAction() { Title = "기능 알려주기", Type = ActionTypes.ImBack, Value = "도움말" },
                    },
                    };
                    await turnContext.SendActivityAsync(welcomeText, cancellationToken);
                    break;

                case 1://도움말
                    var binText = turnContext.Activity.CreateReply("채팅창에 \"안녕\", \"목적 알려주세요\", \"도움말\"을 입력해보세요!");
                    binText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "인사하기", Type = ActionTypes.ImBack, Value = "안녕" },
                        new CardAction() { Title = "목적말하기", Type = ActionTypes.ImBack, Value = "목적 알려주세요" },
                        new CardAction() { Title = "기능 알려주기", Type = ActionTypes.ImBack, Value = "도움말" },
                    },
                    };
                    await turnContext.SendActivityAsync(binText, cancellationToken);
                    break;

            }

        }

    }

}
