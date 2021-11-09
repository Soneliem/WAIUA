


$(document).ready(function() {
  var download_count = 0;
  var output;
  const settings = {
    "async": true,
    "crossDomain": true,
    "url": "https://api.github.com/repos/soneliem/waiua/releases",
    "method": "GET",
    "headers": {}
  };
  
  $.ajax(settings).done(function (response) {
    response.forEach(element => {
      download_count = download_count + element.assets[0].download_count;
    });
    $("#download_count").html(download_count);
    console.log(download_count);
  });


    setInterval(function(){
        $('.about-text > h1 > span').toggleClass('hover');
    },5000);


  });