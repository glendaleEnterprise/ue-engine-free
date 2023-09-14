export function colorHextoRGB(value) {
  let color = value.toLowerCase();
  //判断第一个字符是否是#
  if (color[0] !== '#') {
    color = '#' + color;
    value = color.toUpperCase();
  }
  //判断剩余的字符是否是十六进制
  for (let i = 1; i < color.length; i += 1) {
    let curChar = color[i];
    //console.log(curChar);
    if (isInHex(curChar) == false) {
      color = color.replace(curChar, '');
      value = color.toUpperCase();
    }
  }
  // 如果只有三位的值，需变成六位，如：#fff => #ffffff
  if (color.length === 4) {
    var colorNew = "#";
    for (let i = 1; i < 4; i += 1) {
      colorNew += color.slice(i, i + 1).concat(color.slice(i, i + 1));
    }
    color = colorNew;
  }
  // 处理六位的颜色值，转为RGB
  var colorChange = [];
  for (let i = 1; i < 7; i += 2) {
    colorChange.push(parseInt("0x" + color.slice(i, i + 2)));
  }
  let rgb = "rgba(" + colorChange.join(",") + ",1)";
  return rgb;
};

function isInHex(c) {
  let hexArr = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'];
  for (let i = 0; i < hexArr.length; i += 1) {
    if (hexArr[i] === c) {
      return true;
    }
  }
  return false;
}

export function genlabID(length) {    //随机生成6位数
  return Number(Math.random().toString().substr(3, length));
}

export function dateFormat(time) {
  var date = new Date(time);
  var year = date.getFullYear();
  /* 在日期格式中，月份是从0开始的，因此要加0
   * 使用三元表达式在小于10的前面加0，以达到格式统一  如 09:11:05
   * */
  var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
  var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
  var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
  var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
  var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
  // 拼接
  return year + "-" + month + "-" + day + " " + hours + ":" + minutes + ":" + seconds;
}

export function _isMobile() {
  let flag = navigator.userAgent.match(/(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i)
  return flag;
}
