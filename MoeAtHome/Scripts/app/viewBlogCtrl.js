
moeathomeApp.controller('viewBlogCtrl', ['$scope', '$http', '$routeParams', '$sce',
    function ($scope, $http, $routeParams, $sce) {
        var date = $routeParams.date;
        var title = $routeParams.title;

        var getBlogUrl = function (date, title) {
            return '/api/blogs/' + date + "/" + title;
        }

        var highlight = function () {
            $('pre').each(function (i, e) {
                hljs.highlightBlock(e);
                var $block = $('<div class="ui code segment"><table><tr><td class="gutter"/><td>'
                    + e.outerHTML + '</td></tr></table></div>');
                $(e).replaceWith($block);
            });
            $('.gutter').each(function (i, e) {
                var $thePre = $('pre', $(e).parent());
                var lineCount = $thePre.height() / parseFloat($thePre.css('line-height'));
                for (var i = 0; i < lineCount; i++) {
                    $(e).append("<div class='line-number'>" + (i + 1) + "</div>");
                }
            });
        };

        var renderContent = function (html) {
            $('.blog-content').html(html);
            highlight();
        };

        var getBlog = function () {
            $http({
                method: 'GET',
                url: getBlogUrl(date, title)
            }).success(function (data, status) {
                if (data != 'null') {
                    document.title = data.title + " - Moe@Home";
                    $scope.blog = data;
                    $('#post').css('display', 'block');
                    renderContent(data.content);
                }
                else {
                    document.title = "没找到这篇文章 - Moe@Home";
                    $('#post-notfound').css('display', 'block');
                }
            }).error(function (data, status) {
                // Some error occurred
            });
        };

        $scope.range = function (n) {
            return new Array(n);
        };

        //加载评论
        var loadComments = function () {
            $scope.comments = [{
                floor: 1,
                author: '岁纳Tomato',
                dateTime: '2014-6-3 18:16:54',
                content: '后哈哈哈哈哈哈哈哈哈哈哈哈哈'
            }, {
                floor: 2,
                author: '帝球',
                dateTime: '2014-6-3 18:19:44',
                content: 'びほびほびほ'
            }
            ];
        };

        $scope.onLoaded = function () {
            getBlog();
            loadComments();
        };

        //评论内容
        $scope.commentContent = '';
        //发表评论
        $scope.postComment = function () {
            alert($scope.commentContent);
            //清空评论内容
            $scope.commentContent = '';
        };
    }]);