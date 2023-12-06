// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/**
 * This method will be called at the start of exports.transform in toc.html.js
 */
exports.preTransform = function (model) {
  var namespaceArry = [];
  transformItem(model, 1);
  return model;

  function transformItem(item, level) {
    if (item.name) {
      if (level == 3) namespaceArry[0] = item.name;
      if (level == 4) namespaceArry[1] = item.name;
      if (level == 5) namespaceArry[2] = item.name;

      if (item.topicHref) {
        if (item.topicHref.indexOf("../") != -1) {
          var startindex = item.topicHref.lastIndexOf("/");
          item.topicHref = item.topicHref.substring(startindex + 1, item.topicHref.length);
        }

        var nindex = item.topicHref.indexOf("/");
        if (nindex != -1) {
          
          if (namespaceArry.length > 0 && namespaceArry[0].indexOf("BD.Common8") != -1)
            item.topicHref = namespaceArry[0] + item.topicHref.substring(nindex, item.topicHref.length);

          else if (namespaceArry.length > 1 && namespaceArry[1].indexOf("BD.Common8") != -1)
            item.topicHref = namespaceArry[1] + item.topicHref.substring(nindex, item.topicHref.length);

          else
            item.topicHref = namespaceArry[2] + item.topicHref.substring(nindex, item.topicHref.length);
        }
      }
    } else {
      item.name = null;
    }
    if (item.items && item.items.length > 0) {
      var length = item.items.length;
      for (var i = 0; i < length; i++) {
        transformItem(item.items[i], level + 1);
      };
    }
  }
}


/**
 * This method will be called at the end of exports.transform in toc.html.js
 */
exports.postTransform = function (model) {
  return model;
}
