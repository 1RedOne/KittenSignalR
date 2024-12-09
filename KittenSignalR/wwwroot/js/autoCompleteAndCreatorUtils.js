console.log("Loading utility functions");

function invokeShoppingMethod(channelUrl, method) {
    const addToCartUrl = `/cart/${method}/${channelUrl}`;
    
    const message = `Would be calling ${addToCartUrl} to shopping cart.`;
    console.log(message);
    alert(message);    

    // Send the PUT request
    fetch(addToCartUrl, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        // Optionally include a body if your API expects data
        body: JSON.stringify({ reason: 'User initiated deletion' }),
    })
        .then(response => {
            if (response.ok) {
                alert('Creator ${method} called successfully!');
                // Optionally refresh the page or redirect
                window.location.reload();
            } else {
                alert('Failed to ${method} this creator.');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while trying to ${method} the creator.');
        });
}

async function getAutocompleteResults(query) {
    const resultsContainer = document.getElementById('autocompleteResults');
    resultsContainer.innerHTML = ""; // Clear previous results

    if (!query) return; // Do nothing if the query is empty

    try {
        // Fetch results from the API
        const response = await fetch(`/api/home/autocomplete?query=${encodeURIComponent(query)}`);
        const results = await response.json();

        // Populate the results container
        results.forEach(result => {
            const li = document.createElement('li');
            li.className = "list-group-item";
            li.textContent = result.channelName;
            li.onclick = () => window.open(result.channelUrl, '_blank'); // Open URL on click
            resultsContainer.appendChild(li);
        });
    } catch (error) {
        console.error("Error fetching autocomplete results:", error);
    }
}

function invokeCreatorMethod(channelUrl, method) {
    const requestUrl = `${channelUrl}/${method}`;

    // Send the PUT request
    fetch(requestUrl, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        // Optionally include a body if your API expects data
        body: JSON.stringify({ reason: 'User initiated ' + method + ' request' }),
    })
        .then(response => {
            if (response.ok) {
                alert('Creator ' + method + ' request called successfully!');
                // Optionally refresh the page or redirect
                window.location.reload();
            } else {
                alert('Failed to ' + method + ' request');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while trying to ' + method + ' request the creator.');
        });
}

console.log("Loading shoppingcart, getautocomplete and also invokeCreatorMethod functions")