
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
         //메시지 작업이 수신됨: 메시지 작엄을 처리하도록 재정의함
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            
            var Text = turnContext.Activity.Text;
            //메시지를 수신하기위해
            var replyText = $"아직 구현되지 않은 부분입니다. \r\n  잘 모르겠다면 \"도움말\"을 입력하세요. \r\n";
            if (!GetDataFromServer())
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("음"), cancellationToken);
            }
            //Reply객체에서 해당 reply 가져오기          
            //해당 reply가 없는 경우 -> echo
            //해당 reply가 있는 경우 -> reply
            if (Text.Equals("안녕"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("안녕하세요, '별다줄봇'을 기획한 신세권 팀 입니다.\r\n 신세권은 신조어와 역세권을 합친말로, 흔히 접하는 신조어들을 손쉽게 찾을 수 있다는 의미입니다. \r\n"), cancellationToken);
            }
            else if (Text.Equals("목적 알려주세요"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("저희는 신조어가 어색한 사람들에게 신조어를 알려주고자 \r\n 신조어 알리미 챗봇을 만들고 있습니다."), cancellationToken);
            }
            else if (Text.Equals("도움말"))
            {
                await SendSuggestedActionsAsync(turnContext, cancellationToken, 1);
            }
            else if(Text.Equals("목록"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("별다줄\r\n얼죽아\r\n얼죽코\r\n졌잘싸\r\nJMT\r\n존맛탱\r\n핑프\r\n총 7개의 신조어를 검색할 수 있습니다.\r\n 원하는 신조어의 단어만 입력하세요"), cancellationToken);
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
                    //메시지를 송신하기위해 turnContext 객체의 SendActivityAsync메소드 사용
                    //아직 구현되지 않은 부분입니다. 출력
                    await turnContext.SendActivityAsync(MessageFactory.Text(replyText+"검색할 데이터가 존재하지 않습니다", replyText), cancellationToken);
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

        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken, int function) //버튼
        {

            switch (function)
            {

                case 0://봇이외의 멤버가 대화에 참가함 ㄴ=> 맨 처음에 띄워짐

                    //이미지
                    var image = ProcessInput(turnContext);

                    image.Text = "'별다줄봇'을 추가해주셔서 감사합니다.";
                    image.Attachments = new List<Attachment>(){ GetInternetAttachment() };
                    await turnContext.SendActivityAsync(image, cancellationToken);



                    var welcomeText = turnContext.Activity.CreateReply("별걸 다 줄이는 요즘 말, 궁금하지 않으신가요? \r\n 저희가 알려드릴게요!");
                    welcomeText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "인사하기", Type = ActionTypes.ImBack, Value = "안녕" },
                        new CardAction() { Title = "목적말하기", Type = ActionTypes.ImBack, Value = "목적 알려주세요" },
                        new CardAction() { Title = "기능 알려주기", Type = ActionTypes.ImBack, Value = "도움말" },
                        new CardAction() { Title = "검색가능한 줄임말", Type = ActionTypes.ImBack, Value = "목록" },
                    },
                    };
                    await turnContext.SendActivityAsync(welcomeText, cancellationToken);
                    break;

                case 1://도움말 => 도움말 버튼을 누르거나 "도움말"이라고 채팅이 오면 띄워짐
                    var binText = turnContext.Activity.CreateReply("채팅창에 \"안녕\", \"목적 알려주세요\", \"도움말\", \"목록\"을 입력해보세요!");
                    binText.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>(){
                        new CardAction() { Title = "인사하기", Type = ActionTypes.ImBack, Value = "안녕" },
                        new CardAction() { Title = "목적말하기", Type = ActionTypes.ImBack, Value = "목적 알려주세요" },
                        new CardAction() { Title = "기능 알려주기", Type = ActionTypes.ImBack, Value = "도움말" },
                        new CardAction() { Title = "검색가능한 줄임말", Type = ActionTypes.ImBack, Value = "목록" },
                    },
                    };
                    await turnContext.SendActivityAsync(binText, cancellationToken);
                    break;

            }

        }
        private static Activity ProcessInput(ITurnContext turnContext) //이미지를 보내기 위해서 별도의 Activity 객체를 만들기위한 작업
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
