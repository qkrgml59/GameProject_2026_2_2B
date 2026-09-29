const{add} = require("./Math.js");

let num = 42;
var name = "TOM";
let isStudent = true;

let color = ["red", "green", "blue"];

let person = {name : "Alice", age : 30};

console.log(add(num,num));

function greet(name)
{
    console.log("Hello" + name + "!");
}

greet(person.name);

//조건문
if (num > 30)
{
    console.log("Number is greater than 30");
}
else
{
    console.log("Number is lower than 30");
}

//반복문
for(var i = 0 ; i < 5; i++)
{
    console.log(i);
}

setTimeout(() =>{
    console.log("Delayed Message 1");
}, 1000);

setTimeout(() => {
    console.log("Delayed Mssage 2");
}, 750);

setTimeout(() =>{
    console.log("Delayd Message 4");
}, 500);