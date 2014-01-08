function KiClient() {
    this.rpc = getJsonRPCProxy();
    this.lastdata_msec = 0;
    this.lastdata = null;
    this.lastdata_max_msec = 100;
}

KiClient.prototype.SetThrottle = function (percent) {
    if (percent > 1) {
        percent = 1;
    }
    if (percent < 0) {
        percent = 0;
    }
    this.rpc.SetThrottle({ Set: percent });
};

KiClient.prototype.SetTrim = function (trim) {
    this.rpc.SetTrim({ Set: trim });
};

KiClient.prototype.ToggleGear = function () {
    this.rpc.ToggleGear();
};

KiClient.prototype.ToggleStage = function () {
    this.rpc.ToggleStage();
};

KiClient.prototype.PollData = function () {
    var nowms = Date.now();
    if (this.lastdata == null || (nowms - this.lastdata_msec) > this.lastdata_max_msec) {
        this.lastdata_msec = nowms;
        this.lastdata = this.rpc.PollData();
    }
    return this.lastdata;
};

KiClient.instance = new KiClient();

function getRPC() {
    return KiClient.instance;
}

function getJsonRPCProxy()
{
    var rpc = new JSONRPCProxy('/json/',
        ['PollData', 'SetTrim', 'SetThrottle', 'ToggleGear', 'ToggleStage']);
    return rpc;
}