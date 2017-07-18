// Base

var toDimension = function (v) {

    if (typeof (v) === 'string' && v.endsWith(v))
        return v;

    if (v >= 0)
        return v + "px";

    return "auto";
};

function str2UTF8(str){  
    var bytes = new Array();   
    var len,c;  
    len = str.length;  
    for(var i = 0; i < len; i++){  
        c = str.charCodeAt(i);  
        if(c >= 0x010000 && c <= 0x10FFFF){  
            bytes.push(((c >> 18) & 0x07) | 0xF0);  
            bytes.push(((c >> 12) & 0x3F) | 0x80);  
            bytes.push(((c >> 6) & 0x3F) | 0x80);  
            bytes.push((c & 0x3F) | 0x80);  
        }else if(c >= 0x000800 && c <= 0x00FFFF){  
            bytes.push(((c >> 12) & 0x0F) | 0xE0);  
            bytes.push(((c >> 6) & 0x3F) | 0x80);  
            bytes.push((c & 0x3F) | 0x80);  
        }else if(c >= 0x000080 && c <= 0x0007FF){  
            bytes.push(((c >> 6) & 0x1F) | 0xC0);  
            bytes.push((c & 0x3F) | 0x80);  
        }else{  
            bytes.push(c & 0xFF);  
        }  
    }  
    return bytes;  
}

function concatenate(resultConstructor, ...arrays) {
    let totalLength = 0;
    for (let arr of arrays) {
        totalLength += arr.length;
    }
    let result = new resultConstructor(totalLength);
    let offset = 0;
    for (let arr of arrays) {
        result.set(arr, offset);
    offset += arr.length;
    }
    return result;
}

// Updaters

var updateMargin = function (n, v) { n.style.margin = v; };
var updateWidth = function (n, v) { n.style.width = toDimension(v); };
var updateHeight = function (n, v) { n.style.height = toDimension(v); };
var updateBorderColor = function (n, v) { n.style.borderColor = v; };
var updateBorderThickness = function (n, v) {
    var values = v.split(" ");
    console.log(">>" + toDimension(values[0]))
    console.log(">>" + toDimension(values[1]))
    console.log(">>" + toDimension(values[2]))
    console.log(">>" + toDimension(values[3]))
    n.style.borderTopWidth = toDimension(values[0]);
    n.style.borderRightWidth = toDimension(values[1]);
    n.style.borderBottomWidth = toDimension(values[2]);
    n.style.borderLeftWidth = toDimension(values[3]);
};

// Elements

class Element {
    constructor(values) {
        var self = this;
        this.Identifier = values.Identifier;
        this.Parent = null;
        this.Name = values.Name;
        this.Margin = values.Margin;
        this.Width = values.Width;
        this.Height = values.Height;
        this.HorizontalAlignment = values.HorizontalAlignment;
        this.VerticalAlignment = values.VerticalAlignment;
        this.updaters = {}

        this.updaters.Margin = updateMargin;
        this.updaters.Width = updateWidth;
        this.updaters.Height = updateHeight;
        this.updaters.VerticalAlignment = function (n, v) { if (n && self.Parent && self.Parent.Orientation === "row") n.style.alignSelf = v; };
        this.updaters.HorizontalAlignment = function (n, v) { if (n && self.Parent && self.Parent.Orientation === "column") n.style.alignSelf = v; };
    }

    findElement(identifier) {
        if (identifier == this.Identifier)
            return this;
        return null;
    }

    update(name, value) {
        var updater = this.updaters[name];
        if (updater) updater(this.dom, value);
    }

    raiseEvent(name, arg) {
        if (!arg) arg = "";
        arg = JSON.stringify(arg);
        var head = new Uint8Array(3);
        head[0] = 2;
        head[1] = this.Identifier;
        head[2] = name.length;

        var bytes = concatenate(Uint8Array, head, str2UTF8(name + arg));

        this.socket.send(bytes);
        console.log("(Client) -> (Server) EVENT : " + bytes);
    }

    createDOM(tag) {
        if (!tag) tag = "div";
        var node = document.createElement(tag);
        node.id = this.Identifier;
        node.visibility = this.Visibility;
        node.classList.add(this.constructor.name);
        node.style.cssText += "margin:" + this.Margin + ";";
        if (this.Width >= 0) node.style.cssText += "width:" + toDimension(this.Width);
        if (this.Height >= 0) node.style.cssText += "height:" + toDimension(this.Height);
        if (this.Parent && this.Parent.Orientation) {
            if (this.Parent.Orientation === "row") {
                node.style.cssText += "align-self:" + this.Parent.VerticalAlignment;
            }
            else {
                node.style.cssText += "align-self:" + this.Parent.HorizontalAlignment;
            }
        }
        this.dom = node;
        return node;
    }

    withSocket(socket) {
        this.socket = socket;
        return this;
    }
}

class Panel extends Element {
    constructor(values) {
        super(values);
        this.children = [];
    }

    findElement(identifier) {
        var found = super.findElement(identifier);

        if (found) return found;

        for (var index = 0; index < this.children.length; index++) {
            var found = this.children[index].findElement(identifier);
            if (found) return found;
        }

        return null;
    }

    add(element) {
        this.children.push(element);
        element.Parent = this;
        element.socket = this.socket;


        return this;
    }

    withSocket(socket) {
        this.children.forEach(function (x) { x.withSocket(socket); });
        return super.withSocket(socket);
    }

    createDOM() {
        var node = super.createDOM();
        node.classList.add("Panel");

        this.children.forEach(function (x) {
            node.appendChild(x.createDOM());
            x.update('VerticalAlignment', x.VerticalAlignment);
            x.update('HorizontalAlignment', x.HorizontalAlignment);
        });

        return node;
    }
}

class Page extends Element {
    constructor(values) {
        super(values);
        this.Content = null;
        //this.Background = values.Background;

        //this.updaters.Background = function(n,v) { n.style.backgroundColor = v; };
    }

    content(element) {
        this.Content = element;
        element.Parent = this;
        element.socket = this.socket;
        return this;
    }

    findElement(identifier) {
        var found = super.findElement(identifier);

        if (found) return found;

        if (this.Content) return this.Content.findElement(identifier);

        return null;
    }

    withSocket(socket) {
        if (this.Content) this.Content.withSocket(socket);
        return super.withSocket(socket);
    }

    createDOM() {
        var node = super.createDOM();
        //node.style.cssText += "background-color:" + this.Background + ";";
        if (this.Content) node.appendChild(this.Content.createDOM());
        return node;
    }
}

// Display

class Rectangle extends Element {
    constructor(values) {
        super(values);
        this.Background = values.Background;

        this.updaters.Background = function (n, v) { n.style.backgroundColor = v; };
    }

    createDOM() {
        var node = super.createDOM();
        node.style.cssText += "background-color:" + this.Background + ";";
        return node;
    }
}

class TextBlock extends Element {
    constructor(values) {
        super(values);
        this.Foreground = values.Foreground;
        this.Text = values.Text;
        this.FontSize = values.FontSize;

        this.updaters.Foreground = function (n, v) { n.style.color = v; };
        this.updaters.Text = function (n, v) { n.innerHTML = v; };
        this.updaters.FontSize = function (n, v) { n.style.fontSize = toDimension(v); };
    }

    createDOM() {
        var node = super.createDOM('p');
        node.style.cssText += "color:" + this.Foreground + ";";
        node.style.cssText += "font-size:" + toDimension(this.FontSize) + ";";
        node.innerHTML = this.Text;
        return node;
    }
}

class ProgressRing extends Element {
    constructor(values) {
        super(values);
        this.Foreground = values.Foreground;

        this.updaters.Foreground = function (n, v) {
            var transparentColor = "rgba(" + v + ", 0.2)";
            var indicator = n.firstChild;
            indicator.style.borderLeftColor = transparentColor;
            indicator.style.borderRightColor = transparentColor;
            indicator.style.borderBottomColor = transparentColor;
            indicator.style.borderTopColor = v;
        };
    }

    createDOM() {
        var node = super.createDOM();
        var indicator = document.createElement("div");

        var splits = this.Foreground.split(",");
        var transparentColor = splits[0] + ", " + splits[1] + ", " + splits[2] + ", 0.2)";
        indicator.style.cssText += "border-top-color: " + this.Foreground + ";";
        indicator.style.cssText += "border-left-color: " + transparentColor + ";";
        indicator.style.cssText += "border-right-color: " + transparentColor + ";";
        indicator.style.cssText += "border-bottom-color: " + transparentColor + ";";
        node.appendChild(indicator);

        return node;
    }
}

class Image extends Element {
    constructor(values) {
        super(values);
        this.Source = values.Source;

        this.updaters.Source = function (n, v) { n.style.backgroundImage = "url('" + v; + "')" };
    }

    createDOM() {
        var node = super.createDOM('div');
        node.style.cssText += "background-image: url('" + this.Source + "');";
        return node;
    }
}

// Controls


class Control extends Element {
    constructor(values) {
        super(values);
        this.IsEnabled = values.IsEnabled;

        this.updaters.IsEnabled = function (n, v) { n.disabled = !v; };
    }

    createDOM(tag) {
        var node = super.createDOM(tag);
        node.disabled = !this.IsEnabled;
        return node;
    }
}

class Button extends Control {
    constructor(values) {
        super(values);
        this.Foreground = values.Foreground;
        this.Text = values.Text;
        this.Background = values.Background;
        this.BorderThickness = values.BorderThickness;
        this.BorderColor = values.BorderColor;

        this.updaters.Foreground = function (n, v) { n.style.color = v; };
        this.updaters.Text = function (n, v) { n.innerHTML = v; };
        this.updaters.BorderColor = updateBorderColor;
        this.updaters.BorderThickness = updateBorderThickness;
    }

    createDOM() {
        var self = this;
        var node = super.createDOM('button');
        var borders = this.BorderThickness.split(" ");
        node.type = 'text';
        node.style.cssText += "color:" + this.Foreground + ";background-color:" + this.Background + ";";
        node.style.cssText += "border-color:" + this.BorderColor + ";";
        node.style.cssText += "border-top-width:" + borders[0] + ";";
        node.style.cssText += "border-right-width:" + borders[1] + ";";
        node.style.cssText += "border-bottom-width:" + borders[2] + ";";
        node.style.cssText += "border-left-width:" + borders[3] + ";";
        node.innerHTML = this.Text;
        node.onclick = function () {
            self.raiseEvent("Click");
        }
        return node;
    }
}

class TextBox extends Control {
    constructor(values) {
        super(values);
        this.Foreground = values.Foreground;
        this.Text = values.Text;
        this.PlaceholderText = values.PlaceholderText;
        this.FontSize = values.FontSize;
        this.BorderThickness = values.BorderThickness;
        this.BorderColor = values.BorderColor;

        this.updaters.Foreground = function (n, v) { n.style.color = v; };
        this.updaters.Text = function (n, v) { n.value = v; };
        this.updaters.PlaceholderText = function (n, v) { n.placeholder = v; };
        this.updaters.Background = function (n, v) { n.style.backgroundColor = v; };
        this.updaters.FontSize = function (n, v) { n.style.fontSize = toDimension(v); };
        this.updaters.BorderColor = updateBorderColor;
        this.updaters.BorderThickness = updateBorderThickness;
    }

    createDOM() {
        var self = this;
        var node = super.createDOM("input");
        var borders = this.BorderThickness.split(" ");
        node.placeholder = this.PlaceholderText;
        node.style.cssText += "color:" + this.Foreground + ";";
        node.style.cssText += "font-size:" + toDimension(this.FontSize) + ";";
        node.style.cssText += "border-color:" + this.BorderColor + ";";
        node.style.cssText += "border-top-width:" + borders[0] + ";";
        node.style.cssText += "border-right-width:" + borders[1] + ";";
        node.style.cssText += "border-bottom-width:" + borders[2] + ";";
        node.style.cssText += "border-left-width:" + borders[3] + ";";
        node.innerHTML = this.Text;
        /*
        Add code here to handle non-english input.
        */
        var inputLock = false;
        node.addEventListener('compositionstart', function () {
            inputLock = true;
        });
        node.addEventListener('compositionend', function () {
            inputLock = false;
        });
        node.addEventListener('input', function () {
            if (!inputLock)
                self.raiseEvent("TextChanged", this.value);
        });
        return node;
    }
}

class ToggleSwitch extends Control {
    constructor(values) {
        super(values);
        this.Foreground = values.Foreground;  // TODO
        this.IsOn = values.IsOn;

        this.updaters.Foreground = function (n, v) { }; // TODO
        this.updaters.IsEnabled = function (n, v) {
            var input = n.firstChild;
            input.disabled = !v;
        };
        this.updaters.IsOn = function (n, v) {
            var input = n.firstChild;
            input.checked = v;
        };
    }

    createDOM() {
        var self = this;
        var node = super.createDOM("label");

        //node.style.cssText += "border-left-width:" + borders[3] + ";";

        var input = document.createElement("input");
        input.type = "checkbox";
        input.checked = this.IsOn;
        input.disabled = !this.IsEnabled;
        input.onchange = function (cb) {
            self.raiseEvent("IsOnChanged", "" + cb.srcElement.checked);
        }
        node.appendChild(input);

        var span = document.createElement("span");
        node.appendChild(span);

        return node;
    }
}

class Slider extends Control {
    constructor(values) {
        super(values);
        this.Foreground = values.Foreground;  // TODO
        this.Value = values.Value;

        this.updaters.Foreground = function (n, v) { }; // TODO
        this.updaters.Value = function (n, v) { input.value = v; };
    }

    createDOM() {
        var self = this;
        var node = super.createDOM("input");
        node.type = "range";
        node.value = this.Value;
        node.onchange = function (cb) {
            self.raiseEvent("ValueChanged", "" + this.value);
        }
        return node;
    }
}

// Panels

class StackPanel extends Panel {

    constructor(values) {
        super(values);
        this.Orientation = values.Orientation;

        this.updaters.Orientation = function (n, v) { n.style.flexDirection = v; };
    }

    createDOM() {
        var self = this;
        var node = super.createDOM("div");
        node.style.cssText += "flex-direction:" + this.Orientation + ";";
        return node;
    }
}

// Virtualization

class ListView extends Element {
    constructor(values) {
        super(values);
        this.Source = values.Source;
        this.ItemTemplate = values.ItemTemplate;
    }

    reloadItem(i) { // TODO recycle elements end update properties
        var itemValues = this.Source[i];
        var item = this.ItemTemplate(itemValues);

    }

    createDOM() {
        var node = super.createDOM("div");
        return node;
    }
}

// Main

DataView.prototype.getUTF8String =  function (offset, length) {
    var out, i, len, c;
    var char2, char3;

    out = "";
    len = length + offset - 1;
    i = offset;
    while(i < len) {
    c = this.getUint8(i++);
    switch(c >> 4)
    { 
      case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7:
        // 0xxxxxxx
        out += String.fromCharCode(c);
        break;
      case 12: case 13:
        // 110x xxxx   10xx xxxx
        char2 = this.getUint8(i++);
        out += String.fromCharCode(((c & 0x1F) << 6) | (char2 & 0x3F));
        break;
      case 14:
        // 1110 xxxx  10xx xxxx  10xx xxxx
        char2 = this.getUint8(i++);
        char3 = this.getUint8(i++);
        out += String.fromCharCode(((c & 0x0F) << 12) |
                       ((char2 & 0x3F) << 6) |
                       ((char3 & 0x3F) << 0));
        break;
    }
    }

    return out;
}; 
 
class Mwm {

    constructor(root) {
        var self = this;
        this.node = null;
        this.root = root;
        this.instructions = {};
        this.registerInstruction(0, 'Navigated', function (content) {
            var page = content.getUTF8String(1, content.byteLength)
            self.loadUI(page);
        });
        this.registerInstruction(1, 'PropertyChanged', function (content) {
            var identifier = content.getInt8(1);
            var propLength = content.getInt8(2);
            var propertyName = content.getUTF8String(3, propLength);
            var value = content.getUTF8String(3 + propLength, content.byteLength - propLength - 2);
            console.log("Element - " + identifier + " (prop: " + propertyName + ") : " + value);
            var found = self.node.findElement(identifier);
            found.update(propertyName, eval(value));
        });
    }

    isConnected() {
        return (this.socket && this.socket.readyState === WebSocket.OPEN);
    }

    registerInstruction(id, name, process) {
        this.instructions[id] = {
            name: name, process: function (content) {
                console.log("(" + name + "{" + id + "}) : " + content);
                process(content);
            }
        };
    }

    loadUI(content) {
        var func = "return " + content + ".withSocket(socket)";
        var nodeFunc = new Function('socket', func);
        var node = nodeFunc(this.socket);
        var dom = node.createDOM();
        var container = document.getElementById(this.root);
        container.innerHTML = '';
        container.appendChild(dom);
        this.node = node;
    }

    navigate(name) {
        if (this.isConnected()) {
            var bytes = new Uint8Array(1 + name.length);
            bytes[0] = 0;
            for (var i = 0; i < name.length; i++) {
                bytes[i + 1] = name.charCodeAt(i);
            }
            this.socket.send(bytes);
            console.log("(Client) -> (Server) NAVIGATE : " + name)
        }
        else {
            this.initialNavigation = name;
        }
    }

    start(url) {
        var mwvm = this;

        // Starting socket
        this.socket = new WebSocket(url);
        this.socket.binaryType = 'arraybuffer';
        this.socket.onopen = function (event) {
            console.log("(Client) : connection opened")
            if (mwvm.onStart) mwvm.onStart();
            if (mwvm.initialNavigation) {
                mwvm.navigate(mwvm.initialNavigation)
            }
        };
        this.socket.onclose = function (event) {
            console.log('(Client) : connection closed. Code: ' + event.code + '. Reason: ' + event.reason)
            if (mwvm.onStop) mwvm.onStop();
            mwvm.socket = null;
            document.getElementById(mwm.root).innerHTML = 'error, try to reload the page';
        };
        //socket.onerror = updateState;
        this.socket.onmessage = function (event) {

            var dv = new DataView(event.data);
            var instructionId = dv.getInt8(0);
            var instruction = mwvm.instructions[instructionId];

            if (instruction) {
                instruction.process(dv);
            }

            console.log("(Client) <- (Server) (" + instruction.name + "): " + event.data)
        };
    }
}