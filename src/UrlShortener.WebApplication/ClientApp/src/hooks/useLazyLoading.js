import { useEffect, useState } from "react";

const useLazyLoading = (fetchData, page, setData, data, dependencies = []) => {
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const handleScroll = () => {
      if (loading || !data.length) {
        return;
      }

      const { scrollTop, scrollHeight, clientHeight } = document.documentElement;
      if (scrollTop + clientHeight >= scrollHeight - 5) {
        setLoading(true);
      }
    };

    setLoading(true);
    let isMounted = true;
    fetchData(page)
      .then((newData) => {
        if (isMounted) {
          setData((prevData) => [...prevData, ...newData]);
          setLoading(false);
        }
      })
      .catch((error) => {
        console.error('Error fetching data:', error);
        setLoading(false);
      });

    window.addEventListener('scroll', handleScroll);
    return () => {
      isMounted = false;
      window.removeEventListener('scroll', handleScroll);
    };
  }, dependencies);

  return { loading };
};

export default useLazyLoading;