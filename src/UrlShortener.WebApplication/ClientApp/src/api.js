const apiUrl = process.env.REACT_APP_URL_SHORTENER_ADDRESS;

async function fetchData(page, limit) {
  try {
    const response = await fetch(apiUrl + `all?page=${page}&limit=${limit}`, {
      method: 'GET',
      headers :{
        'Content-Type': 'application/json'
      },
      referrerPolicy: "unsafe-url"
    });
    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching data:', error);
    return [];
  }
}

async function createShortUrl(url) {
  try {
      const response = await fetch(apiUrl + `shorten`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      referrerPolicy: "unsafe-url",
      body: JSON.stringify({ url }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error('Error creating short URL:', error);
    throw error;
  }
}

export { fetchData, createShortUrl };