--[[
    HTTP request sample - Automate Engine
]]

-- create http instance and setup request headers
req = http:create();
headers = {
    UserAgent = "Automate Engine"
}

-- send HTTP request using the send_request() method
result = req:send_request("http://localhost", "get", nil, headers); -- uri, method, requestBody, headers

-- if system error occurred, property has_syserror will be set to true
if result.has_syserror then
    print(result.error);
else
    print("Ok");
end
