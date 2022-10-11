self.onmessage = function (e) {
	console.log('data :' + e.data);


	var data = e.data;
	console.log(data);

	//출력 : 해당 URL들의 내용
	var contents = {};

	//각 URL에 대한
	//HTTP 요청을 시작한다.
	var xhr = new XMLHttpRequest();
	//마지막 인자를 false로 정하여 요청을 동기 방식으로 보낸다.
	xhr.open("POST", data.url, false);
	//응답이 완료될 때까지 대기한다.
	xhr.send(data.req);
	//요청이 실패하면 오류를 발생시킨다.
	if (xhr.status !== 200) {
		throw Error(xhr.status + " " + xhr.statusText + ": " + url);
	}
	//성공했다면 URL의 내용을 저장한다.
	contents = xhr.responseText;
	
	postMessage(contents);
}