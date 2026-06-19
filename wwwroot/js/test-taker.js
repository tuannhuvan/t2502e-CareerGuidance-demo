// Loads test questions and handles submission for the test-taking page.

function loadTestQuestions(testId) {
    $.getJSON('/Tests/GetQuestions', { testId: testId })
        .done(function (questions) {
            var container = $('#questionsContainer');
            container.empty();

            if (!questions || questions.length === 0) {
                container.html('<div class="alert alert-warning">Bài test này chưa có câu hỏi.</div>');
                $('#submitBtn').prop('disabled', true);
                return;
            }

            questions.forEach(function (q, index) {
                var card = $('<div class="card mb-3"></div>');
                var body = $('<div class="card-body"></div>');
                body.append('<h6 class="card-title">Câu ' + (index + 1) + ': ' + escapeHtml(q.content) + '</h6>');

                var inputType = (q.type === 'MultiChoice') ? 'checkbox' : 'radio';
                (q.options || []).forEach(function (opt) {
                    var optId = 'q' + q.id + '_o' + opt.id;
                    var wrapper = $('<div class="form-check"></div>');
                    wrapper.append(
                        '<input class="form-check-input answer-option" type="' + inputType + '" ' +
                        'name="question_' + q.id + '" id="' + optId + '" ' +
                        'data-question-id="' + q.id + '" value="' + opt.id + '">');
                    wrapper.append('<label class="form-check-label" for="' + optId + '">' + escapeHtml(opt.content) + '</label>');
                    body.append(wrapper);
                });

                card.append(body);
                container.append(card);
            });
        })
        .fail(function () {
            $('#questionsContainer').html('<div class="alert alert-danger">Không thể tải câu hỏi. Vui lòng thử lại.</div>');
        });
}

function submitTest() {
    var testId = parseInt($('input[name="testId"]').val(), 10);
    var answers = [];

    $('.answer-option:checked').each(function () {
        answers.push({
            questionId: parseInt($(this).data('question-id'), 10),
            optionId: parseInt($(this).val(), 10)
        });
    });

    if (answers.length === 0) {
        alert('Vui lòng trả lời ít nhất một câu hỏi trước khi nộp bài.');
        return;
    }

    var token = $('input[name="__RequestVerificationToken"]').val();
    $('#submitBtn').prop('disabled', true).text('Đang chấm điểm...');

    $.ajax({
        url: '/Tests/Submit',
        type: 'POST',
        contentType: 'application/json',
        headers: { 'RequestVerificationToken': token },
        data: JSON.stringify({ testId: testId, answers: answers })
    }).done(function (response) {
        if (response && response.success) {
            window.location.href = '/Tests/Results?id=' + response.resultId;
        } else {
            alert((response && response.message) || 'Không thể nộp bài.');
            $('#submitBtn').prop('disabled', false).text('Submit Test');
        }
    }).fail(function (xhr) {
        var msg = 'Không thể nộp bài. Vui lòng thử lại.';
        if (xhr.responseJSON && xhr.responseJSON.message) {
            msg = xhr.responseJSON.message;
        }
        alert(msg);
        $('#submitBtn').prop('disabled', false).text('Submit Test');
    });
}

function escapeHtml(text) {
    if (text == null) return '';
    return $('<div></div>').text(text).html();
}
