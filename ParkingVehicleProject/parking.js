let pageNumber = 1;

function allocateParking() {
    debugger;
    const carPlateNumber = document.getElementById('car-plate-number').value;

    $.ajax({
        url: '/Parking/Allocate',
        type: 'POST',
        data: JSON.stringify({ carPlateNumber }),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            const resultDiv = document.getElementById('allocation-result');
            if (data.ParkingNumber) {
                resultDiv.innerHTML = `Allocated Parking: ${data.ParkingNumber} from ${data.StartDateTime}`;
            } else {
                resultDiv.innerHTML = data.message;
            }
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function loadMore() {
    pageNumber++;
    $.ajax({
        url: `/Parking/Index?pageNumber=${pageNumber}`,
        type: 'GET',
        success: function (data) {
            const recordsDiv = document.getElementById('records');
            data.forEach(record => {
                const recordDiv = document.createElement('div');
                recordDiv.innerHTML = `Car: ${record.CarPlateNumber}, Parking: ${record.ParkingNumber}, Start: ${record.StartDateTime}, End: ${record.EndDateTime}, Amount: ${record.TotalAmount}`;
                recordsDiv.appendChild(recordDiv);
            });
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}
