import * as signalR from "@microsoft/signalr";

class DefaultRetryPolicy implements signalR.IRetryPolicy {

    nextRetryDelayInMilliseconds(retryContext: signalR.RetryContext): number | null {
        console.log("第 " + retryContext.previousRetryCount + "次重试");
        return 3 * 1000;
    }

}

const DocsPage = () => {

    const url = "https://localhost:5076/test";

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(url, {
            // skipNegotiation: true,
            // transport: signalR.HttpTransportType.WebSockets,
        })
        .withHubProtocol(new signalR.JsonHubProtocol())
        //.withAutomaticReconnect(new DefaultRetryPolicy())
        .withAutomaticReconnect({
            nextRetryDelayInMilliseconds(retryContext) {
                return 3 * 1000;
            },
        })
        .build();

    connection.onreconnecting((() => {
        console.log("重连中!");
    }));

    connection.onreconnected(() => {
        console.log("已重连!");
    });

    connection.start().catch((err) => {
        alert(err);
    });

    const onClick = async () => {

        if (connection.state != signalR.HubConnectionState.Connected) {
            createMsgTr("已断线,请稍后再试!");
            return;
        }

        var textDom = document.getElementById("textt") as HTMLInputElement;

        const invokeServerMethod = connection.invoke<string>("ReturnClientResult", textDom?.value);

        invokeServerMethod.then((result) => {
            createMsgTr(result);
        });
    };

    const createMsgTr = (msg: string) => {
        var tbDom = document.getElementById("mainBody");

        var tr = document.createElement("tr");
        var td1 = document.createElement("td");
        td1.innerText = new Date().toString();
        var td2 = document.createElement("td");
        td2.innerText = msg;

        tr.appendChild(td1);
        tr.appendChild(td2);

        tbDom?.append(tr);
    }

    return (
        <div>
            <div>服务端消息</div>
            <table>
                <thead>
                    <tr>
                        <th>时间</th>
                        <th>内容</th>
                    </tr>
                </thead>
                <tbody id="mainBody">

                </tbody>
            </table>

            <input type="text" width={200} id="textt" />
            <button onClick={onClick}>发送消息</button>
        </div>
    );
};

export default DocsPage;
