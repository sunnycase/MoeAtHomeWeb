// Write your Javascript code.

(function () {
    var viewModel = {
        links: [
        {
            url: "http://www.frimin.com",
            name: "Frimin"
        }, {
            url: "http://www.bakachu.cn",
            name: "Chu's box"
        }]
    };

    ko.applyBindings(viewModel);
})();