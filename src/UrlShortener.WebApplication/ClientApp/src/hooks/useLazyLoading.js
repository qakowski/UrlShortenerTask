import {useEffect, useState} from "react";

const useLazyLoading = (fetchData, page, dependencies = []) => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
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

    return () => {
      isMounted = false;
    };
  }, dependencies);

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

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, [data, loading]);

  return { data, loading };
};

export default useLazyLoading;