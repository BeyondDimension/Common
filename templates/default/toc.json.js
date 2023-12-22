// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

exports.transform = function (model) {
    if (model.memberLayout === 'SeparatePages') {
        model = transformMemberPage(model);
    }

    transformModel(model);

    for (var key in model) {
        if (key[0] === '_') {
            delete model[key]
        }
    }
    return {
        content: JSON.stringify(model)
    };
}

function transformModel(model) {
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
                    item.href = item.topicHref.substring(startindex + 1, item.topicHref.length);
                }

                var nindex = item.topicHref.indexOf("/");
                if (nindex != -1) {

                    if (namespaceArry.length > 0 && namespaceArry[0].indexOf("BD.Common8") != -1)
                        item.href = namespaceArry[0] + item.href.substring(nindex, item.href.length);

                    else if (namespaceArry.length > 1 && namespaceArry[1].indexOf("BD.Common8") != -1)
                        item.href = namespaceArry[1] + item.href.substring(nindex, item.href.length);
                    
                    else if(namespaceArry.length > 2)
                        item.href = namespaceArry[2] + item.href.substring(nindex, item.href.length);
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

function transformMemberPage(model) {
    var groupNames = {
        "constructor": { key: "constructorsInSubtitle" },
        "field": { key: "fieldsInSubtitle" },
        "property": { key: "propertiesInSubtitle" },
        "method": { key: "methodsInSubtitle" },
        "event": { key: "eventsInSubtitle" },
        "operator": { key: "operatorsInSubtitle" },
        "eii": { key: "eiisInSubtitle" },
    };

    groupChildren(model);
    transformItem(model, 1);
    return model;

    function groupChildren(item) {
        if (!item || !item.items || item.items.length == 0) {
            return;
        }
        var grouped = {};
        var items = [];
        item.items.forEach(function (element) {
            groupChildren(element);
            if (element.type) {
                var type = element.isEii ? "eii" : element.type.toLowerCase();
                if (!grouped.hasOwnProperty(type)) {
                    if (!groupNames.hasOwnProperty(type)) {
                        groupNames[type] = {
                            name: element.type
                        };
                        console.log(type + " is not predefined type, use its type name as display name.")
                    }
                    grouped[type] = [];
                }
                grouped[type].push(element);
            } else {
                items.push(element);
            }
        }, this);

        // With order defined in groupNames
        for (var key in groupNames) {
            if (groupNames.hasOwnProperty(key) && grouped.hasOwnProperty(key)) {
                items.push({
                    name: model.__global[groupNames[key].key] || groupNames[key].name,
                    items: grouped[key]
                })
            }
        }

        item.items = items;
    }

    function transformItem(item, level) {
        // set to null in case mustache looks up
        item.topicHref = item.topicHref || null;
        item.tocHref = item.tocHref || null;
        item.name = item.name || null;
        item.level = level;
        if (item.items && item.items.length > 0) {
            item.leaf = false;
            var length = item.items.length;
            for (var i = 0; i < length; i++) {
                transformItem(item.items[i], level + 1);
            };
        } else {
            item.items = [];
            item.leaf = true;
        }
    }
}
